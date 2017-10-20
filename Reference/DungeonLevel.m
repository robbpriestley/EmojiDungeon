#import <Foundation/NSException.h> 

#import "DungeonLevel.h"
#import "DungeonSetup.h"
#import "DungeonCoords.h"
#import "DungeonCellTypes.h"

@implementation DungeonLevel

@synthesize level;
@synthesize buildTime;
@synthesize buildPasses;

#pragma mark Public Methods


// init: receives a couple of "global" objects owned by the parent dungeon.
- (id) init:(DungeonCoords*)start typesIn:(DungeonCellTypes*)typesIn empty:(DungeonCell*)empty
{
    if (self = [super init])
    {
        t = typesIn;
        emptyCell = empty;
        buildPasses = 0;
        maxDistance = labs(sqrt(pow(DUNGEON_WIDTH - 1, 2) + pow(DUNGEON_HEIGHT - 1, 2)));
        
        BOOL levelComplete = NO;
        
        do 
        {
            @try 
            {
                buildPasses++;
           
                [self initializeDungeon:start];
                [self generateLevel];
                [self levelSolve];
                levelComplete = YES;  // i.e. no exceptions...
            }
            @catch (DungeonLevelGenerateException* e) 
            {
                levelComplete = NO;   // Try again.
            }
            
        } while (!levelComplete);
    }
    
    [self visualizeAsText];
    
    return self;
}


- (DungeonCell*) dungeonCellAt:(int)X Y:(int)Y
{
    return [dungeonLevel objectAtIndex:(DUNGEON_WIDTH * X) + Y];
}


#pragma mark Initialize


- (void) initializeDungeon:(DungeonCoords*)start
{
    // Create "3-D" dungeon array, it's actually just a "smart indexed" 2-D array.
    dungeonLevel = [[NSMutableArray alloc] initWithCapacity:DUNGEON_WIDTH * DUNGEON_HEIGHT];
    
    // Fill in each cell with the "empty cell" object.
    for(int i = 0; i < DUNGEON_WIDTH * DUNGEON_HEIGHT; i++) 
        [dungeonLevel addObject:emptyCell];
    
    if (start == nil) 
    {
        startCoords = [self placeEntrance];
    }
    else
    {
        // Place a random cell in the previous start position.
        startCoords = start;
        
        NSMutableArray* types = [t getTypes:startCoords];
        
        DungeonCell* newCell = nil;
        DungeonCellType *newType;
        
        while (newCell == nil) 
        {
            newType = [self randomDungeonCellType:types];
            newCell = [[DungeonCell alloc]init:startCoords.X Y:startCoords.Y typeIn:newType];
        }
        
        [self setDungeonCellValue:startCoords.X Y:startCoords.Y cell:newCell];
    }
}


- (void) initializePathSearch
{
    for (int X = 0; X < DUNGEON_WIDTH; X++)
    {
        for (int Y = 0; Y < DUNGEON_HEIGHT; Y++) 
        {
            DungeonCell* cell = [self dungeonCellAt:X Y:Y];
            cell.visited = NO;
            cell.sourceCoords = nil;
        }
    }
}


// Set start cell at bottom centre.
- (DungeonCoords*) placeEntrance
{
    int X = DUNGEON_WIDTH / 2;
    DungeonCellType* startType = t.deadU;
    DungeonCell* entrance = [[DungeonCell alloc] init:X Y:0 typeIn:startType];
    [self setDungeonCellValue:X Y:0 cell:entrance];
    return [[DungeonCoords alloc]init:X Y:0];
}
         

#pragma mark Level


- (void) setDungeonCellValue:(int)X Y:(int)Y cell:(DungeonCell*)cell
{
    int i = (DUNGEON_WIDTH * X) + Y;
    [dungeonLevel replaceObjectAtIndex:i withObject:cell];
    [self recordNewAttachment:cell];
}


- (void) generateLevel
{    
    DungeonCell* cell;
    
    BOOL modified = NO;
    
    // As long as the dungeon is not considered complete, keep adding stuff to it.
    do 
    {
        modified = NO;
        
        for (int Y = 0; Y < DUNGEON_HEIGHT; Y++) 
        {
            for (int X = 0; X < DUNGEON_WIDTH; X++) 
            {
                cell = [self dungeonCellAt:X Y:Y];
                
                if (![cell.type isEmpty] && !cell.attachBlocked && cell.availableConnections > 0)
                {
                    // Attach a new random cell to current cell, if possible. If the cell has 
                    // available connections but nothing can be added to it, consider it blocked.
                    if ([self AttachNewCell:cell]) 
                        modified = YES;
                    else  
                        cell.attachBlocked = YES;
                }
            }
        }
    } while (![self completeCheck:modified]);
}


// If the dungeon level was modified on the last pass, it cannot yet be considered 
// complete. If it was not modified on the last pass, check to see if the dungeon 
// level is filled to completion. If it is not, modify a cell to allow the dungeon 
// to grow some more.
- (BOOL) completeCheck:(BOOL)modified
{
    BOOL complete = NO;
    
    if (!modified)
    {
        int percentFilled = [self calcPercentFilled];
        
        if (percentFilled >= 100)
            complete = YES;
        else
        {
            if (![self forceGrowth])  // Modify a random cell to allow more growth.
            {
                // Sometimes, forceGrowth doesn't work to fill in all the cells in the dungeon level.
                // Frequently, this is because several rooms "block" growth into empty "pockets" near
                // the edge. One option at this point would be to perform some sort of additional 
                // "dungeon augmentation" by swapping corridor types for empty cells. But, that would 
                // involve a bunch of programming. Before I resort to that, I'm going to see how
                // feasible it is to simply abandon this "failed" dungeon level and start fresh...
                [DungeonLevelGenerateException raise:nil format:nil];
            }
        }
    }
    return complete;
}


- (BOOL) AttachNewCell:(DungeonCell*)cell
{
    BOOL attachSuccessful = NO;
    DungeonCoords* coords = [self randomAttachCoords:cell];
    
    if (coords != nil)
    {
        // Get a disposable array of constructed corridor dungeon cell types.
        NSMutableArray* types = [t getTypes:coords];
        
        // Choose a new cell type to attach.
        DungeonCell* newCell = nil;
        DungeonCellType *newType;
        
        while (newCell == nil) 
        {
            if ([types count] == 0) 
                return NO;  // Whoops, there are no more possibilities.
            
            newType = [self randomDungeonCellType:types];
        
            // The new cell needs to be compatible with each adjacent cell.
            if ([self typeCompatibleWithAdjacentCells:newType coords:coords]) 
                newCell = [[DungeonCell alloc]init:coords.X Y:coords.Y typeIn:newType];
        }
        
        [self setDungeonCellValue:coords.X Y:coords.Y cell:newCell];
        attachSuccessful = YES;
    }
    
    return attachSuccessful;
}


// Find which locations adjacent to the current cell could be populated with a new
// attaching cell. Then, out of those locations, select one at random and return the 
// coords for it. If no such location exists, return nil.
- (DungeonCoords*) randomAttachCoords:(DungeonCell*)cell
{
    NSMutableArray* coordPotentials = [[NSMutableArray alloc] init];
    
    // Check each direction for an adjacent cell. Obviously, an adjacent cell has 
    // to be within the coordinate bounds of the dungeon. If the current cell has the 
    // capability to join with a cell in the adjacent location, and the cell in the 
    // adjacent location is empty, add the coords for each such location to the 
    // coordPotentials array. Then choose one of those potential coords at random and 
    // return it. If there are no potentials at all, return nil.
    
    // Cell above.
    if (cell.type.connectsUp && cell.Y + 1 < DUNGEON_HEIGHT)
        if ([[self dungeonCellAt:cell.X Y:cell.Y + 1].type isEmpty])
            [coordPotentials addObject:[[DungeonCoords alloc] init:cell.X Y:cell.Y + 1]];
    
    // Cell below.
    if (cell.type.connectsDown && cell.Y - 1 >= 0)
        if ([[self dungeonCellAt:cell.X Y:cell.Y - 1].type isEmpty])
            [coordPotentials addObject:[[DungeonCoords alloc] init:cell.X Y:cell.Y - 1]];
    
    // Cell left.
    if (cell.type.connectsLeft && cell.X - 1 >= 0)
        if ([[self dungeonCellAt:cell.X - 1 Y:cell.Y].type isEmpty])
            [coordPotentials addObject:[[DungeonCoords alloc] init:cell.X - 1 Y:cell.Y]];
    
    // Cell right.
    if (cell.type.connectsRight && cell.X + 1 < DUNGEON_WIDTH)
        if ([[self dungeonCellAt:cell.X + 1 Y:cell.Y].type isEmpty])
            [coordPotentials addObject:[[DungeonCoords alloc] init:cell.X + 1 Y:cell.Y]];
    
    if ([coordPotentials count] == 0)
        return nil;
    else
    {
        int randomIndex = arc4random() % [coordPotentials count];
        return [coordPotentials objectAtIndex:randomIndex];
    }
}


// With "coords" representing the new, empty dungeon location, check that each of the 
// adjacent cells is compatible with the proposed (randomly determined) new cell type.
- (BOOL) typeCompatibleWithAdjacentCells:(DungeonCellType*)newCellType coords:(DungeonCoords*)coords
{
    // This is an innocent-until-proven guilty scenario. However, if any of the cells
    // is proven to be incompatible, that's enough to eliminate it as a prospect.
    
    DungeonCell *cellUp, *cellDown, *cellLeft, *cellRight;
    
    if (coords.Y + 1 < DUNGEON_HEIGHT)
    {
        cellUp = [self dungeonCellAt:coords.X Y:coords.Y + 1];
        if (![newCellType compatibleWith:cellUp.type direction:Up]) 
            return NO;
    }
    
    if (coords.Y - 1 >= 0)
    {
        cellDown = [self dungeonCellAt:coords.X Y:coords.Y - 1];
        if (![newCellType compatibleWith:cellDown.type direction:Down]) 
            return NO;
    }
    
    if (coords.X - 1 >= 0)
    {
        cellLeft = [self dungeonCellAt:coords.X - 1 Y:coords.Y];
        if (![newCellType compatibleWith:cellLeft.type direction:Left]) 
            return NO;
    }
    
    if (coords.X + 1 < DUNGEON_WIDTH)
    {
        cellRight = [self dungeonCellAt:coords.X + 1 Y:coords.Y];
        if (![newCellType compatibleWith:cellRight.type direction:Right]) 
            return NO;
    }
    
    return YES;
}


// When a new cell is placed in the dungeon, "record" it as such by decrementing the
// availableConnections count of each adjacent, non-empty cell that connects to it.
// Also, decrement the availableConnections count of the new cell accordingly.
- (void) recordNewAttachment:(DungeonCell*)cell
{
    DungeonCell *cellUp, *cellDown, *cellLeft, *cellRight;
    
    if (cell.Y + 1 < DUNGEON_HEIGHT)
    {
        cellUp = [self dungeonCellAt:cell.X Y:cell.Y + 1];
        if ([cell.type connectsTo:cellUp.type direction:Up]) 
        {
            cell.availableConnections--;
            cellUp.availableConnections--;
        }
    }
    
    if (cell.Y - 1 >= 0)
    {
        cellDown = [self dungeonCellAt:cell.X Y:cell.Y - 1];
        if ([cell.type connectsTo:cellDown.type direction:Down]) 
        {
            cell.availableConnections--;
            cellDown.availableConnections--;
        }
    }
    
    if (cell.X - 1 >= 0)
    {
        cellLeft = [self dungeonCellAt:cell.X - 1 Y:cell.Y];
        if ([cell.type connectsTo:cellLeft.type direction:Left])
        {
            cell.availableConnections--;
            cellLeft.availableConnections--;
        }
    }
    
    if (cell.X + 1 < DUNGEON_WIDTH)
    {
        cellRight = [self dungeonCellAt:cell.X + 1 Y:cell.Y];
        if ([cell.type connectsTo:cellRight.type direction:Right]) 
        {
            cell.availableConnections--;
            cellRight.availableConnections--;
        }
    }
}


// Replace a random, non-empty, compatible cell with a different cell to see if that makes the
// dungeon level grow any bigger. But, don't waste any cycles doing it, it could be a lost cause...
- (BOOL) forceGrowth
{
    BOOL success = NO;
    BOOL typeMatch = NO;
    
    DungeonCellType *newType;
    NSMutableArray* cells = [self forceGrowthCells];
    
    while (!success && [cells count] > 0) 
    {
        DungeonCell* cell = [self randomForceGrowthCell:cells];
        DungeonCoords* coords = [[DungeonCoords alloc]init:cell.X Y:cell.Y];
        
        // Attempt to replace it from the standard types.
        NSMutableArray* types = [t getTypes:coords];
        [types removeObject:cell.type];  // ...but replace it with something different.
        
        while (!typeMatch) 
        {
            if ([types count] == 0) 
                break;  // If nothing replaces it, start over.
            
            newType = [self randomDungeonCellType:types];  // Candidate new cell type.
            
            // The new cell needs to be compatible with each adjacent cell.
            if ([self typeCompatibleWithAdjacentCells:newType coords:coords])
            {
                // It is? Cool.
                typeMatch = YES;
            }
        }
        
        if (typeMatch) 
        {
            // Now set the new cell.
            DungeonCell* newCell = 
            [[DungeonCell alloc]init:coords.X Y:coords.Y typeIn:newType];
            [self setDungeonCellValue:coords.X Y:coords.Y cell:newCell];
            success = YES;
        }
    }
    return success;
}


// Gets all the cells in the dungeon that are not empty, and forceGrowthCompatible.
- (NSMutableArray*) forceGrowthCells
{
    DungeonCell* cell;
    NSMutableArray* cells = [[NSMutableArray alloc]init];
    
    for (int X = 0; X < DUNGEON_WIDTH; X++)
    {
        for (int Y = 0; Y < DUNGEON_HEIGHT; Y++) 
        {
            cell = [self dungeonCellAt:X Y:Y];
            if (cell.type.forceGrowthCompatible && ![cell.type isEmpty]) 
            {
                [cells addObject:cell];
            }
        }
    }
    
    return cells;
}


- (DungeonCell*) randomForceGrowthCell:(NSMutableArray*)forceGrowthCells
{
    // Pick a cell randomly, and also eliminate it as a future candidate...
    
    DungeonCell* cell;
    cell = [forceGrowthCells objectAtIndex:arc4random() % [forceGrowthCells count]];
    [forceGrowthCells removeObject:cell];
    return cell;
}


#pragma mark Level Solve


// Ensure that every cell in the dungeon is "reachable". If not, start fresh.
// Also, check for anomalous disconnected cells. If any are found, scrap the level.
- (void) levelSolve;
{
    sequenceNumber = 0;
    
    [self solve:[self dungeonCellAt:startCoords.X Y:startCoords.Y]];
    
    for (int Y = 0; Y < DUNGEON_HEIGHT; Y++) 
        for (int X = 0; X < DUNGEON_WIDTH; X++)
            if (![[self dungeonCellAt:X Y:Y] isVisited])
                [DungeonLevelGenerateException raise:nil format:nil];
    
    [self circularPassagewayCheck];
}


- (void) solve:(DungeonCell*)cell
{
    sequenceNumber++;
    cell.sequence = sequenceNumber;
    cell.visited = YES;
    
    // Cell above.
    if (cell.type.traversableUp && cell.Y + 1 < DUNGEON_HEIGHT)
    {
        DungeonCell* cellAbove = [self dungeonCellAt:cell.X Y:cell.Y + 1];
        // [self connectsCheck:cell otherCell:cellAbove direction:Up];
        if (!cellAbove.visited)
            [self solve:cellAbove];
    }
    
    // Cell below.
    if (cell.type.traversableDown && cell.Y - 1 >= 0)
    {
        DungeonCell* cellBelow = [self dungeonCellAt:cell.X Y:cell.Y - 1];
        // [self connectsCheck:cell otherCell:cellBelow direction:Down];
        if (!cellBelow.visited)
            [self solve:cellBelow];
    }
    
    // Cell left.
    if (cell.type.traversableLeft && cell.X - 1 >= 0)
    {
        DungeonCell* cellLeft = [self dungeonCellAt:cell.X - 1 Y:cell.Y];
        // [self connectsCheck:cell otherCell:cellLeft direction:Left];
        if (!cellLeft.visited)
            [self solve:cellLeft];
    }
    
    // Cell right.
    if (cell.type.traversableRight && cell.X + 1 < DUNGEON_WIDTH)
    {
        DungeonCell* cellRight = [self dungeonCellAt:cell.X + 1 Y:cell.Y];
        // [self connectsCheck:cell otherCell:cellRight direction:Right];
        if (!cellRight.visited)
            [self solve:cellRight];
    }
}


- (void) connectsCheck:(DungeonCell*)cell otherCell:(DungeonCell*)otherCell direction:(Dir)direction
{
    if (![cell.type connectsTo:otherCell.type direction:direction])
    {
        [DungeonLevelGenerateException raise:nil format:nil];  // Scrap it.
    }
}


- (double) distanceFromStartCell:(DungeonCell*)cell
{
    // distance = SQRT[(x2 - x1)^2 + (y2 - y1)^2]
    return labs(sqrt(pow(cell.X - startCoords.X, 2) + pow(cell.Y - startCoords.Y, 2)));
}


- (void) circularPassagewayCheck
{
    for (int Y = 0; Y < DUNGEON_HEIGHT - 1; Y++) 
    {
        for (int X = 0; X < DUNGEON_WIDTH - 1; X++)
        {
            DungeonCell* cell1 = [self dungeonCellAt:X Y:Y];
            DungeonCell* cell2 = [self dungeonCellAt:X Y:Y + 1];
            DungeonCell* cell3 = [self dungeonCellAt:X + 1 Y:Y + 1];
            DungeonCell* cell4 = [self dungeonCellAt:X + 1 Y:Y];
            
            if 
                (
                 [cell1.type connectsTo:cell2.type direction:Up] &&
                 [cell2.type connectsTo:cell3.type direction:Right] &&
                 [cell3.type connectsTo:cell4.type direction:Down] &&
                 [cell4.type connectsTo:cell1.type direction:Left]
                 )
            {
                [DungeonLevelGenerateException raise:nil format:nil];
            }
        }
    }
}

#pragma mark Utility


// Returns a random number between 0 and 100, representing percent.
- (int) randomPercent
{
    return arc4random() % 100;
}

// Returns a random cell from the dungeon level. If empty == YES, then the random
// cell will be empty. If empty == NO, then the random cell will be occupied.
- (DungeonCoords*) randomCell:(BOOL)empty
{
    DungeonCoords* coords = nil;
    
    while (coords == nil) 
    {
        int X = (arc4random() % DUNGEON_WIDTH);
        int Y = (arc4random() % DUNGEON_HEIGHT);
        
        DungeonCell* cell = [self dungeonCellAt:X Y:Y];
        
        if (empty)  // Cell must be occupied.
        {
            if ([cell.type isEmpty]) 
            {
                coords = [[DungeonCoords alloc]init:X Y:Y];
            }
        }
        else  // Cell must be occupied.
        {
            if (![cell.type isEmpty]) 
            {
                coords = [[DungeonCoords alloc]init:X Y:Y];
            }
        }
    }
    
    return coords;
}


- (DungeonCellType*) randomDungeonCellType:(NSMutableArray*)cellTypeArray
{
    // Pick a cell type randomly, and also eliminate it as a candidate for the current
    // cell to avoid re-testing it in the future if it is rejected. DungeonCellTypes
    // have weights, so some are more likely to be picked than others.
    
    int total = 0;
    
    for (DungeonCellType* type in cellTypeArray) 
        total += type.weight;
    
    int threshold = arc4random() % total;
    
    DungeonCellType* type = nil;
    
    for (type in cellTypeArray) 
    {
        threshold -= type.weight;
        
        if (threshold < 0)
        {
            [cellTypeArray removeObject:type];
            break;
        }
    }
    
    return type;
}


- (int) calcPercentFilled
{
    int filledCellCount = 0;
    
    for (int X = 0; X < DUNGEON_WIDTH; X++)
    {
        for (int Y = 0; Y < DUNGEON_HEIGHT; Y++) 
        {
            if (![[self dungeonCellAt:X Y:Y].type isEmpty])
                filledCellCount++;
        }
    }
    
    return (filledCellCount * 100) / (DUNGEON_WIDTH * DUNGEON_HEIGHT);
}


#pragma mark Visualize As Text


- (void) visualizeAsText
{
    DungeonCell *cell;
    NSString *padding;
    NSMutableString *line = [NSMutableString string];
    
    printf("%s", "\n");  // Initial spacer.
    
    int X;
    
    // Because it is console printing, start with the "top" of the dungeon, and work down.
    for (int Y = DUNGEON_HEIGHT - 1; Y >= 0; Y--) 
    {
        for (X = 0; X < DUNGEON_WIDTH * 2; X++) 
        {
            cell = [self dungeonCellAt:X/2 Y:Y];
            
            if (X%2 == 0)
                [line appendString:cell.type.textRep]; 
            else
                [line appendString:cell.type.textRep2];
        }
        
        padding = (Y < 10) ? @"0" : @"";  // For co-ordinate printing.
        
        printf("%s%i%s\n", [padding UTF8String], Y, [line UTF8String]);
        
        if (Y >= 0)
            line = [NSMutableString string];
    }
    
    // Now print X co-ordinate names at the bottom.
    
    printf("%s", "  ");
    
    for (X = 0; X < DUNGEON_WIDTH * 2; X++) 
    {
        if (X % 2 == 0)
            printf("%i", (X/2)/10);
        else
            printf("%s", " "); 
    }
    
    printf("%s", "\n  ");
    
    for (X = 0; X < DUNGEON_WIDTH * 2; X++) 
    {
        if (X % 2 == 0)
            printf("%i", (X/2) % 10);
        else
            printf("%s", " "); 
    }
    
    printf("%s", "\n\n");
}


@end
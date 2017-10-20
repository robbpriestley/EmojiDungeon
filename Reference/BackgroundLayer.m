#import "Queue.h"
#import "MazeScene.h"
#import "DungeonManager.h"
#import "BackgroundLayer.h"


@implementation BackgroundLayer


-(id) init
{
	if ((self = [super init]))
	{
        maze = [(DungeonManager*)[DungeonManager sharedManager] dungeon];
        
        if (retina4)
        {
            self.position = CGPointMake(0, 44);
            
//            CGSize size = [[CCDirector sharedDirector] winSize];
//            
//            CCSpriteFrame* borderTopFrame = [[maze utilityFrameCache] spriteFrameByName:@"Border.png"];
//            CCSpriteFrame* borderBottomFrame = [[maze utilityFrameCache] spriteFrameByName:@"Border.png"];
//            
//            // Hide at first. See [self borderEffect]
//            
//            borderTop = [CCSprite spriteWithSpriteFrame:borderTopFrame];
//            borderTop.anchorPoint = CGPointMake(0.0, 0.0);
//            borderTop.position = CGPointMake(0, size.height);
//            
//            borderBottom = [CCSprite spriteWithSpriteFrame:borderBottomFrame];
//            borderBottom.anchorPoint = CGPointMake(0.0, 0.0);
//            borderBottom.position = CGPointMake(0, -132);
        }
        
        box2LeftPosition = CGPointMake(163, 107);
        box2RightPosition = CGPointMake(163 - 156, 107);
        powerBarBGLeftPosition = CGPointMake(166, 110);
        powerBarBGRightPosition = CGPointMake(166 - 156, 110);
        
        [self addChild:[maze mazeTiles]];
        
        CCSpriteFrame* box1Frame = [[maze utilityFrameCache] spriteFrameByName:@"Box1.png"];
        CCSpriteFrame* box2Frame = [[maze utilityFrameCache] spriteFrameByName:@"Box2.png"];
        CCSpriteFrame* levelBGFrame = [[maze utilityFrameCache] spriteFrameByName:@"PowerBarBG.png"];
        
        // Score grey background.
        box1 = [CCSprite spriteWithSpriteFrame:box1Frame];
        box1.anchorPoint = CGPointMake(0.0, 0.0);
        box1.position = CGPointMake(6, 60);
        [self addChild:box1];
        
        // Power level grey background.
        box2 = [CCSprite spriteWithSpriteFrame:box2Frame];
        box2.anchorPoint = CGPointMake(0.0, 0.0);
        [self addChild:box2];
                
        powerBarBG = [CCSprite spriteWithSpriteFrame:levelBGFrame];
        powerBarBG.anchorPoint = CGPointMake(0.0, 0.0);
        [self addChild:powerBarBG];
        
        [self setAttackButtonPosition];
	}
    
	return self;
}


- (void) setAttackButtonPosition
{
    if (!optionButtonSwitch)
    {
        box2.position = box2LeftPosition;
        powerBarBG.position = powerBarBGLeftPosition;
    }
    else
    {
        box2.position = box2RightPosition;
        powerBarBG.position = powerBarBGRightPosition;
    }
}


- (void) levelUp:(int)powerPoints
{
    [self removeAllChildrenWithCleanup:YES];
    [self addChild:[maze mazeTiles]];
    [self addChild:box1];
    [self addChild:box2];
    [self addChild:powerBarBG];
    [self updatePowerPoints:powerPoints forceDraw:YES];
}


- (void) fadeOut
{
    [self removeChild:box2 cleanup:YES];
    [self removeChild:powerBarBG cleanup:YES];
    
    if (powerBarSprites != nil)
    {
        for (CCSprite* sprite in powerBarSprites)
        {
            [self removeChild:sprite cleanup:YES];
        }
    }
    
//    for (CCSprite* sprite in [maze mazeTiles].children)
//    {
//        id fadeOut = [CCFadeOut actionWithDuration:1.0];
//        [sprite runAction:[CCSequence actions:fadeOut, nil]];
//    }
}


// Depict the path as a quick, fading pulse to let the player know where robot is heading.
- (void) showPath:(CGPoint)sourceTileCoords path:(NSMutableArray*)path
          sprites:(NSMutableArray*)sprites isEnemy:(BOOL)isEnemy;
{
    NSString* frameName = nil;
    sprites = [[NSMutableArray alloc]init];
    
    DungeonCoords *prevCoords = nil, *nextCoords = [path objectAtIndex:[path count] - 1];
    
    GLubyte opacity;
    
    // The path is recorded in reverse, so iterate through it backwards.
    for (int i = [path count] - 1; i >= 0; i--) 
    {
        frameName = nil;
        DungeonCoords* currentCoords = [path objectAtIndex:i];
        
        // Get the previous coords, provided there are some. If there aren't, it's the source.
        if (i < (int)[path count] - 1)
            prevCoords = [path objectAtIndex:i + 1];
        else 
            prevCoords = [[DungeonCoords alloc]init:sourceTileCoords.x Y:sourceTileCoords.y];
        
        // Get the next coords, provided there are some. If there aren't, it the dest.
        if (i > 0)
            nextCoords = [path objectAtIndex:i - 1];
        else 
            frameName = [self pathDestFrameName:prevCoords currentCoords:currentCoords isEnemy:isEnemy];
        
        // If there isn't a frame name yet, as is most often the case, it's somewhere mid-path.
        if (frameName == nil)
        {
            frameName = [self pathNextFrameName:prevCoords currentCoords:currentCoords
                                     nextCoords:nextCoords];
        }
        
        if (i >= (int)[path count] - 8)
        {
            int factor = [path count] - i;
            opacity = (255 / 8) * factor + 2;
        }
        else 
        {
            opacity = 255;
        }
        
        [self addPathElement:frameName X:currentCoords.X Y:currentCoords.Y opacity:opacity sprites:sprites];
    }
}


- (NSString*) pathDestFrameName:(DungeonCoords*)prevCoords currentCoords:(DungeonCoords*)currentCoords
                        isEnemy:(BOOL)isEnemy
{
    NSString* frameName;
    NSString* prefix = @"enemy";
    
    if (!isEnemy)
        prefix = @"robot";
    
    if (prevCoords.X == currentCoords.X)
    {
        if (prevCoords.Y > currentCoords.Y)
            frameName = [prefix stringByAppendingString:@"PathU.png"];
        else 
            frameName = [prefix stringByAppendingString:@"PathD.png"];
    }
    else 
    {
        if (prevCoords.X > currentCoords.X)
            frameName = [prefix stringByAppendingString:@"PathR.png"];
        else 
            frameName = [prefix stringByAppendingString:@"PathL.png"];
    }
    
    return frameName;
}


- (NSString*) pathNextFrameName:(DungeonCoords*)prevCoords currentCoords:(DungeonCoords*)currentCoords
                     nextCoords:(DungeonCoords*)nextCoords
{
    NSString* frameName;
    
    // Prefix: a holdover from when there were separate sprites for the enemy path, too.
    NSString* prefix = @"robot";
    
    if (prevCoords.X == currentCoords.X && currentCoords.X == nextCoords.X)
    {
        frameName = [prefix stringByAppendingString:@"PathUD.png"];
    }
    else if (prevCoords.Y == currentCoords.Y && currentCoords.Y == nextCoords.Y)
    {
        frameName = [prefix stringByAppendingString:@"PathLR.png"];
    }
    else if (prevCoords.X < currentCoords.X && currentCoords.X == nextCoords.X)
    {
        if (currentCoords.Y > nextCoords.Y) 
            frameName = [prefix stringByAppendingString:@"PathDL.png"];
        else 
            frameName = [prefix stringByAppendingString:@"PathUL.png"];
    }
    else if (prevCoords.Y < currentCoords.Y && currentCoords.Y == nextCoords.Y)
    {
        if (currentCoords.X > nextCoords.X) 
            frameName = [prefix stringByAppendingString:@"PathDL.png"];
        else 
            frameName = [prefix stringByAppendingString:@"PathDR.png"];
    }
    else if (prevCoords.X > currentCoords.X && currentCoords.X == nextCoords.X)
    {
        if (currentCoords.Y > nextCoords.Y) 
            frameName = [prefix stringByAppendingString:@"PathDR.png"];
        else 
            frameName = [prefix stringByAppendingString:@"PathUR.png"];
    }
    else if (prevCoords.Y > currentCoords.Y && currentCoords.Y == nextCoords.Y)
    {
        if (currentCoords.X > nextCoords.X) 
            frameName = [prefix stringByAppendingString:@"PathUL.png"];
        else 
            frameName = [prefix stringByAppendingString:@"PathUR.png"];
    }
    
    return frameName;
}


- (void) addPathElement:(NSString*)frameName X:(int)X Y:(int)Y
                opacity:(GLubyte)opacity sprites:(NSMutableArray*)sprites
{
    CGPoint screenCoords = [MazeScene screenCoordsFromTileCoords:X Y:Y];
    
    CCSpriteFrame* frame = [[maze utilityFrameCache] spriteFrameByName:frameName];
    CCSprite* pathElement = [CCSprite spriteWithSpriteFrame:frame];
    pathElement.position = screenCoords;
    pathElement.opacity = opacity;
    
    // Using CCFadeTo instead of CCFadeOut does not override opacity to 100%.
    CCFiniteTimeAction* fadeTo = [CCFadeTo actionWithDuration:1.0f opacity:0];
    [pathElement runAction:fadeTo];
    
    [sprites addObject:pathElement];
    [self addChild:pathElement];
}


- (void) hidePath:(NSMutableArray*)sprites
{
    for (CCSprite* pathElement in sprites)
        [self removeChild:pathElement cleanup:YES];
}


#pragma mark Path


- (NSMutableArray*) path:(CGPoint)currentCoords destX:(int)destX destY:(int)destY
{
    // Breadth-first search to find at least one of the possible shortest paths.
    
    [maze.level initializePathSearch];
    
    DungeonCoords* sourceCoords = [[DungeonCoords alloc]init:currentCoords.x Y:currentCoords.y];
    
    NSMutableArray* queue = [[NSMutableArray alloc]init];
    
    DungeonCell* sourceCell = [maze.level dungeonCellAt:sourceCoords.X Y:sourceCoords.Y];
    sourceCell.visited = YES;
    
    [queue enqueue:sourceCell];
    
    BOOL arrived = NO;
    
    do 
    {
        DungeonCell* cell = [queue dequeue];
        DungeonCoords* sourceCoords = [[DungeonCoords alloc]init:cell.X Y:cell.Y];
        
        if (cell.X == destX && cell.Y == destY)
        {
            arrived = YES; 
        }
        else 
        {
            // Visit all the adjacent cells to the current one, if possible, and in a random sequence.
            // Randomness is required or else for paths of equal lengths, certain favoured directions
            // and patterns will become evident during game play.
            [self visitAdjacents:cell sourceCoords:sourceCoords queue:queue];
        }
        
    } while (!arrived);
    
    // Search complete. Now decode and return results.
    return [self pathSearchDecode:destX destY:destY];
}


- (void) visitAdjacents:(DungeonCell*)cell sourceCoords:(DungeonCoords*)coords queue:(NSMutableArray*)queue
{
    int visitationProfile = arc4random() % 24;
    
    switch (visitationProfile) 
    {
        case 0:
            // UDLR
            [self pathVisitUp:cell sourceCoords:coords queue:queue];
            [self pathVisitDown:cell sourceCoords:coords queue:queue];
            [self pathVisitLeft:cell sourceCoords:coords queue:queue];
            [self pathVisitRight:cell sourceCoords:coords queue:queue];
            break;
            
        case 1:
            // UDRL
            [self pathVisitUp:cell sourceCoords:coords queue:queue];
            [self pathVisitDown:cell sourceCoords:coords queue:queue];
            [self pathVisitRight:cell sourceCoords:coords queue:queue];
            [self pathVisitLeft:cell sourceCoords:coords queue:queue];
            break;
            
        case 2:
            // ULDR
            [self pathVisitUp:cell sourceCoords:coords queue:queue];
            [self pathVisitLeft:cell sourceCoords:coords queue:queue];
            [self pathVisitDown:cell sourceCoords:coords queue:queue];
            [self pathVisitRight:cell sourceCoords:coords queue:queue];
            break;
            
        case 3:
            // ULRD
            [self pathVisitUp:cell sourceCoords:coords queue:queue];
            [self pathVisitLeft:cell sourceCoords:coords queue:queue];
            [self pathVisitRight:cell sourceCoords:coords queue:queue];
            [self pathVisitDown:cell sourceCoords:coords queue:queue];
            break;
            
        case 4:
            // URDL
            [self pathVisitUp:cell sourceCoords:coords queue:queue];
            [self pathVisitRight:cell sourceCoords:coords queue:queue];
            [self pathVisitDown:cell sourceCoords:coords queue:queue];
            [self pathVisitLeft:cell sourceCoords:coords queue:queue];
            break;
            
        case 5:
            // URLD
            [self pathVisitUp:cell sourceCoords:coords queue:queue];
            [self pathVisitRight:cell sourceCoords:coords queue:queue];
            [self pathVisitLeft:cell sourceCoords:coords queue:queue];
            [self pathVisitDown:cell sourceCoords:coords queue:queue];
            break;
            
        case 6:
            // DURL
            [self pathVisitDown:cell sourceCoords:coords queue:queue];
            [self pathVisitUp:cell sourceCoords:coords queue:queue];
            [self pathVisitRight:cell sourceCoords:coords queue:queue];
            [self pathVisitLeft:cell sourceCoords:coords queue:queue];
            break;
            
        case 7:
            // DULR
            [self pathVisitDown:cell sourceCoords:coords queue:queue];
            [self pathVisitUp:cell sourceCoords:coords queue:queue];
            [self pathVisitLeft:cell sourceCoords:coords queue:queue];
            [self pathVisitRight:cell sourceCoords:coords queue:queue];
            break;
            
        case 8:
            // DLRU
            [self pathVisitDown:cell sourceCoords:coords queue:queue];
            [self pathVisitLeft:cell sourceCoords:coords queue:queue];
            [self pathVisitRight:cell sourceCoords:coords queue:queue];
            [self pathVisitUp:cell sourceCoords:coords queue:queue];
            break;
            
        case 9:
            // DLUR
            [self pathVisitDown:cell sourceCoords:coords queue:queue];
            [self pathVisitLeft:cell sourceCoords:coords queue:queue];
            [self pathVisitUp:cell sourceCoords:coords queue:queue];
            [self pathVisitRight:cell sourceCoords:coords queue:queue];
            break;
            
        case 10:
            // DRLU
            [self pathVisitDown:cell sourceCoords:coords queue:queue];
            [self pathVisitRight:cell sourceCoords:coords queue:queue];
            [self pathVisitLeft:cell sourceCoords:coords queue:queue];
            [self pathVisitUp:cell sourceCoords:coords queue:queue];
            break;
            
        case 11:
            // DRUL
            [self pathVisitDown:cell sourceCoords:coords queue:queue];
            [self pathVisitRight:cell sourceCoords:coords queue:queue];
            [self pathVisitUp:cell sourceCoords:coords queue:queue];
            [self pathVisitLeft:cell sourceCoords:coords queue:queue];
            break;
            
        case 12:
            // LUDR
            [self pathVisitLeft:cell sourceCoords:coords queue:queue];
            [self pathVisitUp:cell sourceCoords:coords queue:queue];
            [self pathVisitDown:cell sourceCoords:coords queue:queue];
            [self pathVisitRight:cell sourceCoords:coords queue:queue];
            break;
            
        case 13:
            // LURD
            [self pathVisitLeft:cell sourceCoords:coords queue:queue];
            [self pathVisitUp:cell sourceCoords:coords queue:queue];
            [self pathVisitRight:cell sourceCoords:coords queue:queue];
            [self pathVisitDown:cell sourceCoords:coords queue:queue];
            break;
            
        case 14:
            // LDUR
            [self pathVisitLeft:cell sourceCoords:coords queue:queue];
            [self pathVisitDown:cell sourceCoords:coords queue:queue];
            [self pathVisitUp:cell sourceCoords:coords queue:queue];
            [self pathVisitRight:cell sourceCoords:coords queue:queue];
            break;
            
        case 15:
            // LDRU
            [self pathVisitLeft:cell sourceCoords:coords queue:queue];
            [self pathVisitDown:cell sourceCoords:coords queue:queue];
            [self pathVisitRight:cell sourceCoords:coords queue:queue];
            [self pathVisitUp:cell sourceCoords:coords queue:queue];
            break;
            
        case 16:
            // LRUD
            [self pathVisitLeft:cell sourceCoords:coords queue:queue];
            [self pathVisitRight:cell sourceCoords:coords queue:queue];
            [self pathVisitUp:cell sourceCoords:coords queue:queue];
            [self pathVisitDown:cell sourceCoords:coords queue:queue];
            break;
            
        case 17:
            // LRDU
            [self pathVisitLeft:cell sourceCoords:coords queue:queue];
            [self pathVisitRight:cell sourceCoords:coords queue:queue];
            [self pathVisitDown:cell sourceCoords:coords queue:queue];
            [self pathVisitUp:cell sourceCoords:coords queue:queue];
            break;
            
        case 18:
            // RULD
            [self pathVisitRight:cell sourceCoords:coords queue:queue];
            [self pathVisitUp:cell sourceCoords:coords queue:queue];
            [self pathVisitLeft:cell sourceCoords:coords queue:queue];
            [self pathVisitDown:cell sourceCoords:coords queue:queue];
            break;
            
        case 19:
            // RUDL
            [self pathVisitRight:cell sourceCoords:coords queue:queue];
            [self pathVisitUp:cell sourceCoords:coords queue:queue];
            [self pathVisitDown:cell sourceCoords:coords queue:queue];
            [self pathVisitLeft:cell sourceCoords:coords queue:queue];
            break;
            
        case 20:
            // RDLU
            [self pathVisitRight:cell sourceCoords:coords queue:queue];
            [self pathVisitDown:cell sourceCoords:coords queue:queue];
            [self pathVisitLeft:cell sourceCoords:coords queue:queue];
            [self pathVisitUp:cell sourceCoords:coords queue:queue];
            break;
            
        case 21:
            // RDUL
            [self pathVisitRight:cell sourceCoords:coords queue:queue];
            [self pathVisitDown:cell sourceCoords:coords queue:queue];
            [self pathVisitUp:cell sourceCoords:coords queue:queue];
            [self pathVisitLeft:cell sourceCoords:coords queue:queue];
            break;
            
        case 22:
            // RLDU
            [self pathVisitRight:cell sourceCoords:coords queue:queue];
            [self pathVisitLeft:cell sourceCoords:coords queue:queue];
            [self pathVisitDown:cell sourceCoords:coords queue:queue];
            [self pathVisitUp:cell sourceCoords:coords queue:queue];
            break;
            
        case 23:
            // RLUD
            [self pathVisitRight:cell sourceCoords:coords queue:queue];
            [self pathVisitLeft:cell sourceCoords:coords queue:queue];
            [self pathVisitUp:cell sourceCoords:coords queue:queue];
            [self pathVisitDown:cell sourceCoords:coords queue:queue];
            break;
    }
}


- (void) pathVisitUp:(DungeonCell*)cell sourceCoords:(DungeonCoords*)coords queue:(NSMutableArray*)queue
{
    if (cell.type.traversableUp && cell.Y + 1 < DUNGEON_HEIGHT)
    {
        DungeonCell* cellAbove = [maze.level dungeonCellAt:cell.X Y:cell.Y + 1];
        if (!cellAbove.visited)
        {
            cellAbove.Visited = YES;
            cellAbove.SourceCoords = coords;
            [queue enqueue:cellAbove];
        }
    }
}


- (void) pathVisitDown:(DungeonCell*)cell sourceCoords:(DungeonCoords*)coords queue:(NSMutableArray*)queue
{
    if (cell.type.traversableDown && cell.Y - 1 >= 0)
    {
        DungeonCell* cellBelow = [maze.level dungeonCellAt:cell.X Y:cell.Y - 1];
        if (!cellBelow.visited)
        {
            cellBelow.Visited = YES;
            cellBelow.SourceCoords = coords;
            [queue enqueue:cellBelow];
        }
    }
}


- (void) pathVisitLeft:(DungeonCell*)cell sourceCoords:(DungeonCoords*)coords queue:(NSMutableArray*)queue
{
    if (cell.type.traversableLeft && cell.X - 1 >= 0)
    {
        DungeonCell* cellLeft = [maze.level dungeonCellAt:cell.X - 1 Y:cell.Y];
        if (!cellLeft.visited)
        {
            cellLeft.Visited = YES;
            cellLeft.SourceCoords = coords;
            [queue enqueue:cellLeft];
        }
    }
}


- (void) pathVisitRight:(DungeonCell*)cell sourceCoords:(DungeonCoords*)coords queue:(NSMutableArray*)queue
{
    if (cell.type.traversableRight && cell.X + 1 < DUNGEON_WIDTH)
    {
        DungeonCell* cellRight = [maze.level dungeonCellAt:cell.X + 1 Y:cell.Y];
        if (!cellRight.visited)
        {
            cellRight.Visited = YES;
            cellRight.SourceCoords = coords;
            [queue enqueue:cellRight];
        }
    }
}


- (NSMutableArray*) pathSearchDecode:(int)destX destY:(int)destY
{
    BOOL pathComplete = NO;
    
    DungeonCell* cell = [maze.level dungeonCellAt:destX Y:destY];
    DungeonCoords* coords = [[DungeonCoords alloc]init:cell.X Y:cell.Y];
    
    NSMutableArray* path = [[NSMutableArray alloc]init];
    [path addObject:coords];
    
    if (cell.sourceCoords == nil)
        return path;  // It's just a shorty.
    
    [path addObject:cell.sourceCoords];
    
    do 
    {
        cell = [maze.level dungeonCellAt:cell.sourceCoords.X Y:cell.sourceCoords.Y];
        
        if (cell.sourceCoords != nil)
        {
            [path addObject:cell.sourceCoords];
        }
        else 
        {
            [path removeLastObject];  // Don't actually need the source location in the path.
            pathComplete = YES;
        }
        
    } while (!pathComplete);
    
    //[self CCLOG_Path:path];
    
    return path;
}


- (void) CCLOG_Path:(NSMutableArray*)path
{
    for (NSUInteger i = 0; i < [path count]; i++) 
    {
        DungeonCoords* coords = [path objectAtIndex:i];
        
        coords = coords; // Shush the compiler warning.
        
        CCLOG(@"%i: %i,%i", i, coords.X, coords.Y);
    }
}


- (void) updatePowerPoints:(int)powerPoints forceDraw:(BOOL)forceDraw
{
    // First, determine if existing power bars need to be removed. If they don't, do nothing.
    
    int powerBars = powerPoints * 20 / 100 + 1;
    
    if (powerBars > 20)
        powerBars = 20;
    else if (powerPoints == 0)
        powerBars = 0;
    
    if (powerBarSprites == nil || powerBars != (int)powerBarSprites.count || forceDraw)
    {
        // Otherwise, remove existing power bars.
        
        if (powerBarSprites != nil)
        {
            for (CCSprite* sprite in powerBarSprites)
            {
                [self removeChild:sprite cleanup:YES];
            }
        }
        
        // Next, draw new ones.
        
        powerBarSprites = [[NSMutableArray alloc]init];
     
        int x = !optionButtonSwitch ? 169 : 169 - 156;

        NSString* powerBarName;

        if (powerPoints > 50)
        powerBarName = @"PowerBarGreen.png";
        else if (powerPoints > 25)
        powerBarName = @"PowerBarOrange.png";
        else
        powerBarName = @"PowerBarRed.png";

        for (int i = 0; i < powerBars; i++)
        {
            CCSpriteFrame* powerBarSpriteFrame = [[maze utilityFrameCache] spriteFrameByName:powerBarName];

            CCSprite* powerBar = [CCSprite spriteWithSpriteFrame:powerBarSpriteFrame];
            powerBar.anchorPoint = CGPointMake(0.0, 0.0);
            powerBar.position = CGPointMake(x, 113);
            [self addChild:powerBar];
            [powerBarSprites addObject:powerBar];
            
            x += 7;
        }
    }
 }


//- (void) borderEffect
//{
//    CGSize size = [[CCDirector sharedDirector] winSize];
//    
//    CGPoint borderTopStart = CGPointMake(0, size.height - 44 * 2);
//    CGPoint borderBottomStart = CGPointMake(0, -44);
//    
//    CGPoint borderTopFinish = CGPointMake(0, size.height);
//    CGPoint borderBottomFinish = CGPointMake(0, -132);
//    
//    id moveTopStart = [CCMoveTo actionWithDuration:0 position:borderTopStart];
//    id moveBottomStart = [CCMoveTo actionWithDuration:0 position:borderBottomStart];
//    
//    id moveTopFinish = [CCMoveTo actionWithDuration:3.0 position:borderTopFinish];
//    id moveBottomFinish = [CCMoveTo actionWithDuration:3.0 position:borderBottomFinish];
//    
//    id topActions = [CCSequence actions: moveTopStart, moveTopFinish, nil];
//    id bottomActions = [CCSequence actions: moveBottomStart, moveBottomFinish, nil];
//    
//    [self addChild:borderTop];
//    [self addChild:borderBottom];
//    [borderTop runAction:topActions];
//    [borderBottom runAction:bottomActions];
//}


@end

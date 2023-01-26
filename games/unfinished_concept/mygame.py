from pygame_functions import *
import math, random

# define colors
red = [255, 0, 0]
green = [0, 255, 0]
blue = [0, 0, 255]
black = [0, 0, 0]
white = [255, 255, 255]
cardColor = [183,240,114]

# create a screen
screenWidth = 1000
screenHeight = 750
win = screenSize(screenWidth, screenHeight)
setBackgroundImage('stars.png')
pygame.display.set_caption('Dungeon Boop')
setAutoUpdate(False)                # allow us to manually update the games frames per second

fpsDisplay = makeLabel("FPS: ", 15, 10, 10, 'white', 'comic sans')
showLabel(fpsDisplay)

hero_sprite = makeSprite('Dungeon_Character.png')
skeleton_sprite = makeSprite('skeletonSprites.png', 4)

########################## Classes #############################################
class player(object):
    def __init__(self, x, y, frames, maxHP):
        self.x = x
        self.y = y
        self.nextFrame = clock()
        self.currentFrame = 0
        self.frame_num = frames
        self.maxHP = maxHP

    # create and show the sprite of the hero, as well as animate
    def drawSprite(self, sprite, x, y):
        addSpriteImage(sprite, 'Dungeon_Character_2.png')
        moveSprite(sprite, x, y, True)
        showSprite(sprite)
        self.animate(sprite)

    def animate(self, sprite):
        if clock() > self.nextFrame:
            self.currentFrame = (self.currentFrame + 1) % self.frame_num
            self.nextFrame += 250
            sprite.changeImage(self.currentFrame)



    #def drawHP(self):
        #hpBar = pygame.draw.rect(win, [255,255,255], (200,550, 650, 150))


class enemy(player):
    pass

    #def animate(self, sprite):
    #    if clock() > self.nextFrame:
    #        self.currentFrame = (self.currentFrame + 1) % self.frame_num
    #        self.nextFrame += 250
    #        sprite.changeImage(self.currentFrame)


# create a class object for the in game hand, default is for the hero character
class hand(object):
    def __init__(self, handSizeMax):
        self.handSizeMax = handSizeMax
        self.hbWidth = 650
        self.hbHeight = 120
        self.hbColor = red
        self.hbLocation = (200, 550)
        self.hbLocationEnemy = (200, 40)
        self.hbRect = (self.hbLocation[0], self.hbLocation[1], self.hbWidth, self.hbHeight)
        self.hbRectEnemy = (self.hbLocationEnemy[0], self.hbLocationEnemy[1], self.hbWidth, self.hbHeight)
        self.cardWidth = 90
        self.cardHeight = 110


    def makeHand(self):
        handBox = pygame.draw.rect(win, self.hbColor, self.hbRect)
        x = 30
        l = [0,1,2,3,4]

        for card in range(self.handSizeMax):
            cardLocation = ((self.hbLocation[0] + (1 + l[card])*x + l[card]*self.cardWidth), self.hbLocation[1] + 5)
            self.makeCard(win, cardColor, cardLocation[0], cardLocation[1], self.cardWidth, self.cardHeight)


    def makeHandEnemy(self, currentHandSize):
        handBoxEnemy = pygame.draw.rect(win, self.hbColor, self.hbRectEnemy)
        x = 30
        l = [0,1,2,3,4]
        handList = list()
        for card in range(currentHandSize):
            cardLocationEnemy = ((self.hbLocationEnemy[0] + (1 + l[card])*x + l[card]*self.cardWidth), self.hbLocationEnemy[1] + 5)
            cardObject = self.makeCard(win, cardColor, cardLocationEnemy[0], cardLocationEnemy[1], self.cardWidth, self.cardHeight)
            handList.append(cardObject)
        if keyPressed('up'):
            pause(1000)
            handList.remove(handList[currentHandSize - 1])
            self.makeHandEnemy(len(handList))
            currentHandSize -= 1
            #print(len(handList))

        #card1 = makeCard(win, cardColor, 205, 555, 90, 110)
        #card2 = makeCard(win, cardColor, 305, 555, 90, 110)



    # draw a rectangle on the window, with color and position as well as width and height
    @staticmethod
    def makeCard(screen, color, xpos, ypos, w, h):
        card = pygame.draw.rect(screen, color, (xpos,ypos,w,h))
        return card


##################################################################################




def displayMousePos():
    mPositionX, mPositionY = pygame.mouse.get_pos()
    xLabel = makeLabel(('X ' + str(object=mPositionX)), 20, 875, 10, 'white', 'arial', 'blue')
    yLabel = makeLabel(('Y ' + str(object=mPositionY)), 20, 950, 10, 'white', 'arial', 'blue')
    showLabel(xLabel), showLabel(yLabel)




def redrawGameWindow():
    hero.drawSprite(hero_sprite, 350, 500)
    skeleton.drawSprite(skeleton_sprite, 700, 200)

    heroHand.makeHand()
    skeletonHand.makeHandEnemy(5)

    #hero.drawHP()
    displayMousePos()
    updateDisplay()

# create our class objects
hero = player(x = 350, y = 500, frames = 2, maxHP = 10)
heroHand = hand(5)
skeleton = enemy(x = 700, y = 200, frames = 4, maxHP = 5)
skeletonHand = hand(5)


################################ MAINLOOP #########################################
while True:


    # handles frame updates and displays frames per second
    fps = tick(30)
    changeLabel(fpsDisplay, 'FPS: {}'.format(str(round(fps, 2))))
    redrawGameWindow()

#################################################################################

endWait()

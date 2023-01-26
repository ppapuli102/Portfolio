import pygame
import random

pygame.init()

bg = pygame.image.load('bground.png')
win = pygame.display.set_mode((720, 640))
#char = [pygame.image.load('Dungeon_Character.png'), pygame.image.load('Dungeon_Character_2.png')]
#char = pygame.transform.scale(char, (64,64))
clock = pygame.time.Clock()

def load_image(name):
    image = pygame.image.load(name)
    return image

class player(object):
    def __init__(self, x, y, width, height):
        self.x = x
        self.y = y
        self.width = width
        self.height = height
        self.images = []
        self.images.append(load_image('Dungeon_Character.png'))
        self.images.append(load_image('Dungeon_Character_2.png'))

        self.index = 0
        self.image = self.images[self.index]
        self.rect = pygame.Rect(5, 5, 64, 64)

    def update(self):
        '''This method iterates through the elements inside self.images and displays the next one
            on each tick. For a slower animmation, you may want to consider using a timer of some
            sort so it updates slower.'''

        self.index += 1
        if self.index >= len(self.images):
            self.index = 0
        self.image = self.images[self.index]


class enemy(object):
    def __init__(self, x, y, width, height):
        self.x = x
        self.y = y
        self.width = width
        self.height = height

def redrawGameWindow():
    win.blit(bg,(0,0))
    hero.draw(win)

    pygame.display.update()



def main():

    hero = player(200, 410, 128,128)
    #my_group = pygame.sprite.Group(hero)

    running = True

    while running:
        for event in pygame.event.get():
            if event.type == pygame.QUIT:
                running = False

        #my_group.update()
        #my_group.draw(win)
        pygame.display.flip()
        redrawGameWindow()

if __name__=="__main__":
    # call the main function
    main()

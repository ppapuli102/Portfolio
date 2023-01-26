import pygame
pygame.init()

clock = pygame.time.Clock()

print('wait 500ms')
pygame.time.wait(500)

print('tick', clock.tick())

print('wait 1250ms')
pygame.time.wait(1250)

print('tick', clock.tick())

print('num of seconds since pygame has been initiated is: ', pygame.time.get_ticks())

print('get_raw_time', clock.get_rawtime())

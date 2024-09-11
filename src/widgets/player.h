#ifndef PLAYER_H
#define PLAYER_H

#include <mpv/client.h>
#include <ncurses.h>

#include "menu.h"

struct player {
    WINDOW *win;

    int height, width;
    int y, x;

    mpv_handle *mpv;
    char name[1024];

    bool paused;
    double prevolume;
};

struct player player_create(int y, int x, int width);
void player_play(struct player *player, struct menu_item *item);
void player_update(struct player *player, int key);
void player_resize(struct player *player, int height, int width);
void player_destroy(struct player *player);

#endif

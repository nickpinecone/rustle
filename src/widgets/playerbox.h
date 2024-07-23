#ifndef PLAYERBOX_H
#define PLAYERBOX_H

#include <ncurses.h>
#include <mpv/client.h>

// TODO transfer mpv handling to here, so it's fully isolated

struct playerbox
{

};

struct playerbox player_create(mpv_handle* mpv);
void player_update(struct playerbox* player);

#endif

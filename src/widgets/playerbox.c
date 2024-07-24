#include <mpv/client.h>
#include <ncurses.h>
#include <string.h>

#include "playerbox.h"

struct playerbox player_create(int y, int x, int width)
{
    mpv_handle *mpv = mpv_create();
    mpv_initialize(mpv);

    WINDOW *win = newwin(3, width, y, x);
    refresh();

    box(win, 0, 0);
    wrefresh(win);

    return (struct playerbox){
        .raw = win, .y = y, .x = x, .width = width, .height = 3, .mpv = mpv, .length = 0, .pause = true};
}

void player_stop(struct playerbox *player)
{
    player->name[0] = '\0';
    player->length = 0;
    player->pause = true;

    const char *args[] = {"stop", NULL};
    mpv_command(player->mpv, args);
}

void player_play(struct playerbox *player, struct selectitem *item)
{
    strcpy(player->name, item->label);
    player->length = item->length;
    player->pause = false;

    const char *args[] = {"loadfile", item->url, NULL};
    mpv_command(player->mpv, args);
}

void _volume_down(struct playerbox *player)
{
    const char *args[] = {"add", "volume", "-1", NULL};
    mpv_command(player->mpv, args);
}

void _volume_up(struct playerbox *player)
{
    const char *args[] = {"add", "volume", "+1", NULL};
    mpv_command(player->mpv, args);
}

void _clear(struct playerbox *player)
{
    char empty[player->width - 2];
    memset(empty, ' ', player->width - 2);
    empty[player->width - 2] = '\0';
    mvwprintw(player->raw, 1, 1, "%s", empty);
    wmove(player->raw, 1, 1);
}

void player_update(struct playerbox *player, int key)
{
    switch (key)
    {
    case ' ':
        player_stop(player);
        break;
    case 'd':
        _volume_down(player);
        break;
    case 'u':
        _volume_up(player);
        break;
    default:
        break;
    }

    _clear(player);
    wprintw(player->raw, "%s ", player->name);
    double result;
    mpv_get_property(player->mpv, "volume", MPV_FORMAT_DOUBLE, &result);
    wprintw(player->raw, "%.f", result);
    wrefresh(player->raw);
}

void player_destroy(struct playerbox *player)
{
    mpv_destroy(player->mpv);
}

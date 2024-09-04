#include <mpv/client.h>
#include <ncurses.h>
#include <string.h>

#include "../utils/config.h"
#include "keycodes.h"
#include "playerbox.h"

void _clear(struct playerbox *player)
{
    char empty[player->width - 2];
    memset(empty, ' ', player->width - 2);
    empty[player->width - 2] = '\0';
    mvwprintw(player->raw, 1, 1, "%s", empty);
    wmove(player->raw, 1, 1);
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

void _stop(struct playerbox *player, bool clear)
{
    if (clear)
    {
        strcpy(player->name, "");
        player->length = 0;
    }

    const char *args[] = {"stop", NULL};
    mpv_command(player->mpv, args);
}

struct playerbox player_create(int y, int x, int width)
{
    mpv_handle *mpv = mpv_create();

    config_init();
    mpv_load_config_file(mpv, config_get_mpv());
    mpv_initialize(mpv);

    WINDOW *win = newwin(3, width, y, x);
    refresh();

    box(win, 0, 0);
    wrefresh(win);

    return (struct playerbox){.raw = win,
                              .y = y,
                              .x = x,
                              .width = width,
                              .height = 3,
                              .mpv = mpv,
                              .length = 0,
                              .pause = true,
                              .prevolume = 100};
}

void player_toggle(struct playerbox *player)
{
    player->pause = !player->pause;
    char volume[1024];

    if (player->pause)
    {
        mpv_get_property(player->mpv, "volume", MPV_FORMAT_DOUBLE, &player->prevolume);
        strcpy(volume, "0");
    }
    else
    {
        sprintf(volume, "%.f", player->prevolume);
    }

    const char *args[] = {"set", "volume", volume, NULL};
    mpv_command(player->mpv, args);
}

void player_play(struct playerbox *player, struct selectitem *item)
{
    _stop(player, false);

    strcpy(player->name, item->label);
    player->length = item->length;
    player->pause = false;

    const char *args[] = {"loadfile", item->url, NULL};
    mpv_command(player->mpv, args);
}

void player_update(struct playerbox *player, int key)
{
    switch (key)
    {
    case KEY_ESCAPE:
        _stop(player, true);
        break;
    case ' ':
        player_toggle(player);
        break;
    case 'n':
        _volume_down(player);
        break;
    case 'p':
        _volume_up(player);
        break;
    default:
        break;
    }

    _clear(player);
    wprintw(player->raw, "%s ", player->name);
    double volume;
    mpv_get_property(player->mpv, "volume", MPV_FORMAT_DOUBLE, &volume);
    wprintw(player->raw, "%.f", volume);
    wrefresh(player->raw);
}

void player_destroy(struct playerbox *player)
{
    delwin(player->raw);
    mpv_destroy(player->mpv);
}

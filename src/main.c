#include <ncurses.h>

#include "utils/main_win.h"
#include "widgets/menu.h"
#include "widgets/player.h"

int main() {
    struct main_win main_win = main_win_init();
    struct menu menu = menu_create(0, 0, main_win.height - 3, main_win.width);
    struct player player = player_create(menu.height, 0, main_win.width);

    int key = 0;

    while (true) {
        struct menu_item *item = menu_update(&menu, key);

        if (item != NULL) {
            player_play(&player, item);
        }

        player_update(&player, key);

        if (key == 'q') {
            break;
        }

        key = getch();
    }

    menu_destroy(&menu);
    return main_win_close();
}

#include <ncurses.h>

#include "utils/main_win.h"
#include "widgets/menu.h"

int main() {
    struct main_win main_win = main_win_init();
    struct menu menu = menu_create(0, 0, main_win.height, main_win.width);

    int key = 0;

    while (true) {
        menu_update(&menu, key);

        if (key == 'q') {
            break;
        }

        key = getch();
    }

    menu_destroy(&menu);
    return main_win_close();
}

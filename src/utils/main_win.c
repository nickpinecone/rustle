#include <ncurses.h>

#include "main_win.h"

void main_win_set_dim(struct main_win *main_win) {
    int y, x;
    getmaxyx(stdscr, y, x);
    main_win->height = y;
    main_win->width = x;
}

struct main_win main_win_init() {
    // Initialize ncurses
    initscr();
    cbreak();
    noecho();
    timeout(-1);
    curs_set(0);
    keypad(stdscr, TRUE);

    struct main_win main_win = (struct main_win){.win = stdscr};
    main_win_set_dim(&main_win);

    return main_win;
}

void main_win_resize(struct main_win *main_win) {
    clear();
    refresh();
    main_win_set_dim(main_win);
}

int main_win_close() {
    endwin();
    return 0;
}

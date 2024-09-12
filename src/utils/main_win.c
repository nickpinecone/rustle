#include <ncurses.h>

#include "main_win.h"

struct main_win main_win_init() {
    // Initialize ncurses
    initscr();
    cbreak();
    noecho();
    timeout(-1);
    curs_set(0);
    keypad(stdscr, TRUE);

    int y, x;
    getmaxyx(stdscr, y, x);

    return (struct main_win){.win = stdscr, .height = y, .width = x};
}

void main_win_resize(struct main_win *main_win) {
    clear();
    refresh();

    int y, x;
    getmaxyx(stdscr, y, x);
    main_win->height = y;
    main_win->width = x;
}

int main_win_close() {
    endwin();
    return 0;
}

#include <ncurses.h>

#include "conf.h"
#include "main_win.h"

void ncurses_init() {
    initscr();
    cbreak();
    noecho();

    timeout(-1);
    curs_set(0);
    keypad(stdscr, TRUE);
}

struct main_win main_win_init() {
    ncurses_init();
    conf_init();

    int y, x;
    getmaxyx(stdscr, y, x);

    return (struct main_win){.win = stdscr, .height = y, .width = x};
}

int main_win_close() {
    endwin();
    return 0;
}

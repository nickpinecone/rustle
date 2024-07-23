#include <ncurses.h>

#include "mainwindow.h"

void ncurses_init()
{
    initscr();
    cbreak();
    noecho();
    keypad(stdscr, TRUE);
    timeout(0);
}

struct mainwindow main_init()
{
    ncurses_init();

    int y, x;
    getmaxyx(stdscr, y, x);

    return (struct mainwindow){.raw = stdscr, .height = y, .width = x};
}

int main_close()
{
    endwin();
    return 0;
}

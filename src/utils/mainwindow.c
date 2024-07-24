#include <ncurses.h>

#include "mainwindow.h"

void ncurses_init()
{
    initscr();
    cbreak();
    noecho();
    
    timeout(-1);
    curs_set(0);
    keypad(stdscr, TRUE);
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

#ifndef MAIN_WIN_H
#define MAIN_WIN_H

#include <ncurses.h>

struct main_win {
    WINDOW *win;

    int height;
    int width;
};

struct main_win main_win_init();
int main_win_close();

#endif

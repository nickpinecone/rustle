#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <ncurses.h>

struct mainwindow 
{
    WINDOW* raw;
    int width, height;
};

struct mainwindow main_init();
int main_close();

#endif

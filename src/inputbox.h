#ifndef INPUTBOX_H
#define INPUTBOX_H

#include <ncurses.h>

enum instate {
    Capture,
    Exit,
};

struct inputbox 
{
    WINDOW *raw;
    int height, width;
    int y, x;

    int inY, inX;
    char content[1024];
    int length;
    bool isFocus;
};

struct inputbox input_create(int height, int width, int y, int x);
void input_box(struct inputbox* input, char* label, int y, int x);
enum instate input_capture(struct inputbox* input);

#endif

#ifndef INPUTBOX_H
#define INPUTBOX_H

#include <ncurses.h>

enum instatus {
    Capture,
    Enter,
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
    bool focus;
};

struct inputbox input_create(int y, int x, int width, char* label);
void input_focus(struct inputbox* input);
enum instatus input_capture(struct inputbox* input, int key);
void input_destroy(struct inputbox* input);

#endif

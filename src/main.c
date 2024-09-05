#include <ncurses.h>

#include "utils/main_win.h"

int main() {
    struct main_win main_win = main_win_init();

    return main_win_close();
}

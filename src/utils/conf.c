#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <sys/stat.h>
#include <sys/types.h>
#include <unistd.h>

#include "conf.h"

char _home_dir[1024];
char _config_dir[1024];
char _mpv_path[1024];
char _conf_path[1024];

char *conf_gen_mpv() {
    return "stream-lavf-o-append=timeout=10000000\n\
stream-lavf-o-append=reconnect_on_http_error=4xx,5xx\n\
stream-lavf-o-append=reconnect_delay_max=30\n\
stream-lavf-o-append=reconnect_streamed=yes\n\
stream-lavf-o-append=reconnect_on_network_error=yes\n";
}

char *conf_gen_riff() {
    return "[\n\
    {\n\
        \"name\": \"Code Radio\",\n\
        \"url\": \"https://coderadio-admin-v2.freecodecamp.org/listen/coderadio/radio.mp3\"\n\
    }\n\
]\n";
}

void conf_init() {
    strcpy(_home_dir, getenv("HOME"));
    sprintf(_config_dir, "%s/.config/riff/", _home_dir);
    sprintf(_mpv_path, "%s/mpv.conf", _config_dir);
    sprintf(_conf_path, "%s/riff.json", _config_dir);

    if (stat(_config_dir, &(struct stat){}) == -1) {
        mkdir(_config_dir, S_IRWXU);

        FILE *mpv_conf = fopen(_mpv_path, "w");
        fprintf(mpv_conf, "%s", conf_gen_mpv());
        fclose(mpv_conf);

        FILE *example_conf = fopen(_conf_path, "w");
        fprintf(example_conf, "%s", conf_gen_riff());
        fclose(example_conf);
    }
}

char *conf_get_mpv() { return _mpv_path; }
char *conf_get_riff() { return _conf_path; }

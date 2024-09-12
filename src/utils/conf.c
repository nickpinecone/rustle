#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <sys/stat.h>
#include <sys/types.h>
#include <unistd.h>

#include "conf.h"

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

struct conf conf_init() {
    char *home_dir = getenv("HOME");
    char *conf_part = "/.config/riff/";
    char *mpv_part = "mpv.conf";
    char *riff_part = "riff.json";

    char *conf_dir =
        malloc(sizeof(char) * (strlen(home_dir) + strlen(conf_part) + 1));
    sprintf(conf_dir, "%s%s", home_dir, conf_part);

    char *mpv_path =
        malloc(sizeof(char) *
               (strlen(home_dir) + strlen(conf_part) + strlen(mpv_part) + 1));
    sprintf(mpv_path, "%s%s%s", home_dir, conf_part, mpv_part);

    char *riff_path =
        malloc(sizeof(char) *
               (strlen(home_dir) + strlen(conf_part) + strlen(riff_part) + 1));
    sprintf(riff_path, "%s%s%s", home_dir, conf_part, riff_part);

    if (stat(conf_dir, &(struct stat){}) == -1) {
        mkdir(conf_dir, S_IRWXU);

        FILE *mpv_conf = fopen(mpv_path, "w");
        fprintf(mpv_conf, "%s", conf_gen_mpv());
        fclose(mpv_conf);

        FILE *riff_conf = fopen(riff_path, "w");
        fprintf(riff_conf, "%s", conf_gen_riff());
        fclose(riff_conf);
    }

    free(conf_dir);

    return (struct conf){
        .mpv_path = mpv_path,
        .riff_path = riff_path,
    };
}

void conf_destroy(struct conf *conf) {
    free(conf->mpv_path);
    free(conf->riff_path);
}

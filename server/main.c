//
//  main.c
//  HelloWorld
//
//  Created by 김준호 on 2014. 10. 2..
//  Copyright (c) 2014년 김준호. All rights reserved.
//

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <arpa/inet.h>

int main(int argc, char * argv[])
{
    int serv_sock;
    int clnt_sock;
    
    struct sockaddr_in serv_addr;
    struct sockaddr_in clnt_addr;
    
    int clnt_addr_size;
    
    char message[] = "hello world!\n";
    
    serv_sock = socket(PF_INET, SOCK_STREAM, 0);
    
    memset(&serv_addr,0,sizeof(serv_addr));
    serv_addr.sin_family = AF_INET;
    serv_addr.sin_addr.s_addr = htonl(INADDR_ANY);
    serv_addr.sin_port = htonl(8000);
    bind(serv_sock, (struct sockaddr *)&serv_addr, sizeof(serv_addr));
    listen(serv_sock,5);
    
    clnt_addr_size = sizeof(clnt_addr);
    clnt_sock = accept(serv_sock, (struct sockaddr *)&clnt_addr,&clnt_addr_size);
    
    write(clnt_sock,message,sizeof(message));
    
    close(clnt_sock);
    
    return 0;
}
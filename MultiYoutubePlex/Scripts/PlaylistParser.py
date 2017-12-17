# Name: PlayListParser.py
# Version: 1.0
# Author: Mitke
# Email: miticm30@gmail.com
# Description: Parse URLs in Youtube User's Playlist and returns new line delimited string
 
import re
import urllib.request
import urllib.error
import sys
import time
 
def crawl(url):
    sTUBE = ''
    cPL = ''
    amp = 0
    final_url = []

    if 'http' not in url:
        url = 'http://' + url

    if 'list=' in url:
        eq = url.rfind('=') + 1
        cPL = url[eq:]
            
    else:
        print('Incorrect Playlist.')
        exit(1)
    
    try:
        yTUBE = urllib.request.urlopen(url).read()
        sTUBE = str(yTUBE)
    except urllib.error.URLError as e:
        print(e.reason)
    
    tmp_mat = re.compile(r'watch\?v=\S+?list=' + cPL)
    mat = re.findall(tmp_mat, sTUBE)
 
    if mat:
          
        for PL in mat:
            yPL = str(PL)
            if '&' in yPL:
                yPL_amp = yPL.index('&')
            final_url.append('http://www.youtube.com/' + yPL[:yPL_amp])
 
        return list(set(final_url)).join('\n');
        
    else:
        print('No videos found.')
        exit(1)
        

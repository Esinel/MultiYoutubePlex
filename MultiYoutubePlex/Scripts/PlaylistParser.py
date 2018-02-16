# Name: PlayListParser.py
# Version: 1.0
# Author: Mitke
# Email: miticm30@gmail.com
# Description: Parse URLs in Youtube User's Playlist and returns new line delimited string
 
import re
import sys
import time

html = ''
final_list = '';

def main():
    sTUBE = ''
    cPL = ''
    amp = 0
    final_url = []
    if 'list=' in url:
        eq = url.rfind('=') + 1
        cPL = url[eq:]
            
    else:
        print('Incorrect Playlist.')
        exit(1)
        
    tmp_mat = re.compile(r'watch\?v=\S+?list=' + cPL)
    mat = re.findall(tmp_mat, html)
 
    if mat:
          
        for PL in mat:
            yPL = str(PL)
            if '&' in yPL:
                yPL_amp = yPL.index('&')
            final_url.append('http://www.youtube.com/' + yPL[:yPL_amp])
 
        final_list = list(set(final_url)).join('\n');
        
    else:
        print('No videos found.')
        exit(1)

if __name__ == "__main__":
	main();
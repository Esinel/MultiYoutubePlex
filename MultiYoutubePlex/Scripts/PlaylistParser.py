# Name: PlayListParser.py
# Version: 1.0
# Author: Mitke
# Email: miticm30@gmail.com
# Description: Parse URLs in Youtube User's Playlist and returns new line delimited string
 #Python 2.7
import re
import urllib2, urllib
import urllib2, urllib
import sys
import time
 
def crawl(url):
    sTUBE = u''
    cPL = u''
    amp = 0
    final_url = []

    if u'http' not in url:
        url = u'http://' + url

    if u'list=' in url:
        eq = url.rfind(u'=') + 1
        cPL = url[eq:]
            
    else:
        print u'Incorrect Playlist.'
        exit(1)
    
    try:
        yTUBE = urllib2.urlopen(url).read()
        sTUBE = unicode(yTUBE)
    except urllib2.URLError, e:
        print e.reason
    
    tmp_mat = re.compile(ur'watch\?v=\S+?list=' + cPL)
    mat = re.findall(tmp_mat, sTUBE)
 
    if mat:
          
        for PL in mat:
            yPL = unicode(PL)
            if u'&' in yPL:
                yPL_amp = yPL.index(u'&')
            final_url.append(u'http://www.youtube.com/' + yPL[:yPL_amp])
 
        return list(set(final_url)).join(u'\n');
        
    else:
        print u'No videos found.'
        exit(1)
        

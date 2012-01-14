FDLManager
==========

FDLManager is an easy to use utility that keeps your HTTP server up to date with the custom maps, models, etc you have on your game server.


Configuration
-------------------

To run FDLManager, you'll need:

 - A Windows-based system with .NET Framework 2.0, or
 - A Linux-based system with Mono 2.10 or above.

A typical configuration for CS:S would look like:

    [mono] FileLister.exe \
        --game cstrike \
        --s ~/gameserver/css/cstrike/maps \     
		--s ~/gameserver/css/cstrike/models \
		--s ~/gameserver/css/cstrike/materials \
 		--s ~/gameserver/css/cstrike/sound \   
		--dest /var/www/fastdl \      
		--list games/css.txt

Adding custom games
--------------------

FDLManager comes with support for CS:S and TF2, but you can easily add more games.

For example, to add DOD:S, you would run the following command **on a vanilla server**:

    [mono] FileLister.exe \
        --game dod \
        --s ~/gameserver/orangebox/dod/maps \
		--s ~/gameserver/orangebox/dod/sound \
		--s ~/gameserver/orangebox/dod/models \ 
		--s ~/gameserver/orangebox/dod/materials \      
		--list games/dod.txt

Basically, it will create a text file in games/dod.txt with a list of all the files that does not need FastDL-ing (the default game content). 

The next time you'll run the command (after adding some maps, models and so forth), it will FastDL the files that are not on the list.

TODO
----

 - FTP support.
 - More games by default.
 - Automatically update game files when new files are added by Valve.
 - Turn this into a service.
 - Add a Python version for people who do not want to install Mono on Linux.
 
License
-------

Copyright (C) 2010 Alon Gubkin

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
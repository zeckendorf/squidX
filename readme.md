## Squid for Everyone!

### About
This is the open-source C# [(mono)](http://www.mono-project.com/) implementation 
of Squid, a twin-stick [shmup](https://en.wikipedia.org/wiki/Shoot_%27em_up) I 
developed a few years ago with a [friend](http:/stevegregory.me). You may 
remember it from its tenure as an Xbox360
[gem](https://www.engadget.com/2011/02/25/xbox-live-indie-gems-squid/). This
project takes on a new life every couple of years as a way to learn a new
platform or language by porting it &mdash; to spread the love, it's here on
github so anyone can use it to get started making their own game.

### Premise
You are Squid &mdash; a cargo shuttle trapped in a space warzone! With no
weapons to speak of, the only option you have is to outmaneuver your enemies
by redirecting enemy fire back into them. Supporting up to 4-player local
co-op, stay alive as long as you can to rack up the high score.

### Requirements & Setup

Originally built with XNA 3.0, this port uses on Mono 5.8 with
[Monogame](http://www.monogame.net/) 3.6. YMMV on a given operating system,
but for Debian based linux such as Ubuntu, you'll need the following to develop

* mono-complete
* libopenal-dev
* referenceassemblies-pcl 

----
sudo apt-get install libopenal-dev 
sudo apt-get install mono-runtime
sudo apt-get install referenceassemblies-pcl

----

You'll also need Monodevelop 6 specifically for Monogame 3.6, so grab it:

----
wget https://github.com/cra0zy/monodevelop-run-installer/releases/download/6.2.0.1778-1/monodevelop-6.2.0.1778-1.run
chmod +x monodevelop-6.2.0.1778-1.run
sudo ./monodevelop-6.2.0.1778-1.run
----

And finally monogame itself:

----
wget http://www.monogame.net/releases/v3.6/monogame-sdk.run
chmod +x monogame-sdk.run
sudo ./monogame-sdk.run
----


### Screenshots

![Menu Screen](https://i.imgur.com/wSChBhJ.png)
![Gameplay](https://i.imgur.com/RsvWSDm.png)
![Game Over](https://i.imgur.com/jAoZf1Y.png)

### License
© 2018 &mdash; Sam Zeckendorf

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

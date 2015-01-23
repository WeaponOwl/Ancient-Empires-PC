%%version = "01a"
%%title = "Ancient Empire II - Map Editor"


title %%title
option decimalsep,","
option fieldsep,"|"
option priority,idle
DIRECTORY change,@path(%0)

if @winexists(%%title)
	window activate,%%title
	stop
end

inifile open,@path(%0)@name(%0)".ini"

%%lastmap = @iniread(autosaved,lastmap,@path(%0)"*.*")
%%grid = @iniread(autosaved,grid,1)
%%newmapsize = @iniread(autosaved,newmapsize,"15 x 15")
%%texsel = @iniread(autosaved,texsel,18)

rem buffer temporaire
list create,1

rem map en memoire ds list 8
list create,8

  DIALOG CREATE,%%title,-1,0,750,579
REM *** Modifié par Dialog Designer le 17/05/2007 - 01:13 ***
  DIALOG ADD,SHAPE,SHAPE1,2,2,638,553,BLACK,BLACK,,RECTANGLE
  dialog hide,SHAPE1
  DIALOG ADD,BUTTON,LOADMAP,120,648,96,24,"Load map"
  DIALOG ADD,TEXT,TEXT1,562,4,,,TEXT1
  DIALOG ADD,BUTTON,SAVEMAP,148,648,96,24,"Save map"
  DIALOG ADD,BUTTON,NEWMAP,176,648,96,24,New map
  DIALOG ADD,CHECK,GRID,206,648,96,18,Show Grid,%%grid,,CLICK
  DIALOG SHOW

  dialog disable,savemap

%%createblocs = 1
%%clearblocs = 1
gosub initscreen

%%actualmapfile = "No map in memory"
%%mapmode = 1
%%OLDmapmode = %%mapmode

rem draw tileset
%t = 0
%u = 0
%v = 0
repeat
	%a = @sum(558,@prod(%t,26))
	%b = @sum(8,@prod(%u,26))
	if @greater(2,@len(%v))
		%v = "0"%v
	end
	if @not(@equal(%v,28))
		DIALOG ADD,BITMAP,"t_"%v,%b,%a,26,26,@path(%0)"AE2map.bin|"@prod(%v,406),,CLICK
	end
	%v = @succ(%v)
	if @greater(%t,1)
		%t = 0
		%u = @succ(%u)
	else
		%t = @succ(%t)
	end
until @greater(%v,46)

rem draw unitset
%t = 0
%u = 0
%v = 0
repeat
	%a = @sum(558,@prod(%t,26))
	%b = @sum(446,@prod(%u,26))
	if @greater(2,@len(%v))
		%v = "0"%v
	end
	if @not(@equal(%v,28))
		DIALOG ADD,BITMAP,"u_"%v,%b,%a,26,26,@path(%0)"AE2map.bin|"@prod(@sum(%v,47),406),,CLICK
	end
	%v = @succ(%v)
	if @greater(%t,1)
		%t = 0
		%u = @succ(%u)
	else
		%t = @succ(%t)
	end
until @equal(%v,12)

rem selection
if %%mapmode
	DIALOG ADD,BITMAP,"s_1",10,652,48,48,@path(%0)"AE2map.bin|"@prod(%%texsel,406),"selected",CLICK,STRETCH
else
	DIALOG ADD,BITMAP,"s_1",10,652,48,48,@path(%0)"AE2map.bin|"@prod(@sum(%%unitsel,47),406),"selected",CLICK,STRETCH
end

dialog show,SHAPE1
:evloop
wait "0,1"
%e = @event()
if %e
	%a = @substr(%e,1,2)
	if @equal(%a,"t_")@equal(%a,"b_")@equal(%a,"u_")@equal(%a,"s_")
		goto BLOCEVENT
	end
	goto %e
end
goto evloop

:close
inifile write,autosaved,lastmap,%%lastmap
inifile write,autosaved,grid,%%grid
inifile write,autosaved,newmapsize,%%newmapsize
inifile write,autosaved,texsel,%%texsel

STOP

:NEWMAPBUTTON
%a = @input("Enter map size in bloc LxH (max : 22. min : 8 for L)",%%newmapsize,)
if @not(@ok())
	goto evloop
end
option fieldsep,"x"
parse "%b;%c",%a
if @not(%c)
	warn "You didn't entered correctly the sizes. New map creation cancelled."
	goto evloop
end
if @greater(8,%b)
	beep
	goto NEWMAPBUTTON
end
if @greater(%c,22)
	beep
	goto NEWMAPBUTTON
end
if @greater(%b,22)
	beep
	goto NEWMAPBUTTON
end
list clear,1

%%mapXbloc = %b
%%mapYbloc = %c

%%newmapsize = %%mapXbloc" x "%%mapYbloc

rem entete
list add,1,0
list add,1,0
list add,1,0
list add,1,%%mapXbloc
list add,1,0
list add,1,0
list add,1,0
list add,1,%%mapYbloc

rem corps
%c = 0
repeat
	%c = @succ(%c)
	list add,1,%%texsel
until @equal(%c,@prod(%%mapXbloc,%%mapYbloc))

rem dummy ? sais pas trop ce que c 'est..
%a = 48
list add,1,0
list add,1,0
list add,1,0
list add,1,%a
%c = 0
repeat
	list add,1,0
	list add,1,0
	list add,1,0
	list add,1,2
	%c = @succ(%c)
until @equal(%c,%a)

rem données unités, ici entete et 0 unité
list add,1,0
list add,1,0
list add,1,0
list add,1,0

%%actualmapfile = "New_Map"
dialog enable,savemap

gosub drawmap
if @not(%%mapmode)
	gosub unitdraw
end

goto evloop


:SAVEMAPBUTTON
%%mapfile = @filedlg("AE2 Map File","Specify filename to save AE2 Map",%%actualmapfile,SAVE)
if @not(%%mapfile)
	goto evloop
end

%%lastmap = %%mapfile

binfile open,1,%%mapfile,CREATE
if @not(@ok())
	warn "Cannot save to : "%%mapfile", file locked ?"
	goto evloop
end
list seek,1,0
repeat
	%t = @next(1)
	if %t
		binfile write,1,binary,%t
	end
until @not(%t)
binfile close,1

%%actualmapfile = %%mapfile

goto evloop

:LOADMAPBUTTON
%%mapfile = @filedlg("AE2 Map file","Select AE2 Map File to load",%%lastmap)
if @not(%%mapfile)
	goto evloop
end

%%lastmap = %%mapfile

if @both(@not(@file(%%mapfile,ahrs)),if @not(@file(%%mapfile)))
	goto evloop
end

%%mapfilesize = @file(%%mapfile,z)

if @not(@greater(%%mapfilesize,0))
	warn "Error, maps didn't seem valid in size!"
	goto evloop
end

if @greater(%%mapfilesize,2048)
	warn "Error, map size seem too big to be a valid file!"
	goto evloop
end

binfile open,1,%%mapfile,read
rem %%mm = @binfile(read,1,hex,%%mapfilesize)
list clear,1
repeat
	%c = @binfile(read,1,binary,1)
	%c = @substr(%c,1,@pred(@len(%c)))
	list add,1,%c
until @binfile(eof,1)
binfile close,1

if @not(@equal(@count(1),%%mapfilesize))
	warn "Internal Error ! map in memory isn't the same length than the file : "@count(1)"/"%%mapfilesize" !"@cr()"Please report to author!"
	list clear,1
	goto evloop
end

list seek,1,0
%%o1 = @next(1)
%%o2 = @next(1)
%%o3 = @next(1)
%%o4 = @next(1)
%i = 0
%i = @sum(@prod(%%o1,16777216),%i)
%i = @sum(@prod(%%o2,65536),%i)
%i = @sum(@prod(%%o3,256),%i)
%i = @sum(%%o4,%i)
%%mapXbloc = %i

list seek,1,4
%%o1 = @next(1)
%%o2 = @next(1)
%%o3 = @next(1)
%%o4 = @next(1)

%i = 0
%i = @sum(@prod(%%o1,16777216),%i)
%i = @sum(@prod(%%o2,65536),%i)
%i = @sum(@prod(%%o3,256),%i)
%i = @sum(%%o4,%i)
%%mapYbloc = %i

if @greater(%%mapXbloc,22)
	warn "Error, map horizontal size seem too larger! : "%%mapXbloc
	goto evloop
end
if @greater(%%mapYbloc,22)
	warn "Error, map vertical size seem too larger! : "%%mapYbloc
	goto evloop
end

%%actualmapfile = %%mapfile
dialog enable,savemap
gosub drawmap
if @not(%%mapmode)
	gosub unitdraw
end
goto evloop


:initscreen
%t = 0
%u = 0
repeat
	if %%grid
		%a = @sum(4,@prod(%t,25))
		%b = @sum(4,@prod(%u,25))
	else
		%a = @sum(4,@prod(%t,24))
		%b = @sum(4,@prod(%u,24))		
	end
	if %%createblocs
		DIALOG ADD,BITMAP,"b_"%t"x"%u,%b,%a,24,24,,,CLICK
	end
	if %%setposblocks
		dialog setpos,"b_"%t"x"%u,%b,%a,24,24
	end
	if %%clearblocs
		dialog hide,"b_"%t"x"%u
	end
	if @greater(%t,20)
		%t = 0
		%u = @succ(%u)
	else
		%t = @succ(%t)
	end
until @greater(%u,21)
%%createblocs =
%%clearblocs =
%%setposblocks =
exit
_

:BLOCEVENT
%a = @substr(%e,1)
if @equal(%a,"t")
	%%BLOCEVENT = @substr(%e,3,4)
	goto BLOCEVENT_T
end
if @equal(%a,"b")
	%%BLOCEVENT = @substr(%e,3,@diff(@len(%e),5))
	goto BLOCEVENT_B
end
if @equal(%a,"u")
	%%BLOCEVENT = @substr(%e,3,@diff(@len(%e),5))
	goto BLOCEVENT_U
end
if @equal(%a,"s")
	goto BLOCEVENT_S
end
warn "Missing code here ;P"
goto evloop

:BLOCEVENT_T
%%texsel = %%BLOCEVENT
dialog set,"s_1",@path(%0)"AE2map.bin|"@prod(%%texsel,406)
%%mapmode = 1
if @not(@equal(%%mapmode,%%OLDmapmode))
	gosub drawmap
end
%%OLDmapmode = %%mapmode
goto evloop

:BLOCEVENT_B
if %%mapmode
	option fieldsep,"X"
	parse "%a;%b",%%BLOCEVENT
	%c = @sum(@prod(%%mapYbloc,%a),%b)
	list seek,1,@sum(%c,8)
	list delete,1
	list insert,1,%%texsel
	rem  - debug only!
	rem gosub drawmap
	dialog set,"B_"%%BLOCEVENT,@path(%0)"AE2map.bin|"@prod(%%texsel,406)
else
	if @equal("LEFT",@click(B))
		gosub UNITADD
	end
	if @equal("RIGHT",@click(B))
		gosub UNITDEL
	end
	if @equal("CENTER",@click(B))
		info TODO UNIT CENTER
	end
	rem info gosub unitdraw
end
goto evloop

:BLOCEVENT_U
%%unitsel = %%BLOCEVENT
%v = @sum(@prod(12,%%unitcolorswitch),%%unitsel)
if @greater(2,@len(%v))
	%v = "0"%v
end
dialog set,"s_1",@path(%0)"AE2map.bin|"@prod(@sum(%v,47),406)
rem info %%BLOCEVENT@cr()%%unitcolorswitch@cr()%%unitsel@cr()@path(%0)"textures\unit_icons_"%%unitsel".bmp"
%%mapmode =
if @not(@equal(%%mapmode,%%OLDmapmode))
	gosub unitdraw
end
%%OLDmapmode = %%mapmode

goto evloop

:BLOCEVENT_S
if %%mapmode
	beep
	rem info TODO for mapmode
	goto evloop
end
%%unitcolorswitch = @succ(%%unitcolorswitch)
if @greater(%%unitcolorswitch,3)
	%%unitcolorswitch = 0
end
%v = @sum(@prod(12,%%unitcolorswitch),%%unitsel)
if @greater(2,@len(%v))
	%v = "0"%v
end
dialog set,"s_1",@path(%0)"AE2map.bin|"@prod(@sum(%v,47),406)
rem info %%BLOCEVENT@cr()%%unitcolorswitch@cr()%%unitsel@cr()@path(%0)"textures\unit_icons_"%%unitsel".bmp"
rem info %%BLOCEVENT
rem dialog set,"s_1",@path(%0)"textures\unit_icons_"%%texsel".bmp"

goto evloop

:drawmap
if @zero(@count(1))
	exit
end
if @equal(%%mapmode,%%OLDmapmode)
	%%clearblocs = 1
	gosub initscreen
end
list seek,1,8
%t = 0
%u = 0
%c = 0
repeat
	%v = @next(1)
	%c = @succ(%c)
	dialog set,"b_"%t"x"%u,@path(%0)"AE2map.bin|"@prod(%v,406)
	dialog show,"b_"%t"x"%u
	if @greater(%u,@diff(%%mapYbloc,2))
		%u = 0
		%t = @succ(%t)
	else
		%u = @succ(%u)
	end
until @equal(%c,@prod(%%mapXbloc,%%mapYbloc))
dialog set,text1,"Map size : "%%mapXbloc" x "%%mapYbloc", "@prod(%%mapXbloc,%%mapYbloc)"blocs, "%%actualmapfile
exit
_

:unitdraw
if @zero(@count(1))
	rem warn cannot drawunit without a map in memory!
	exit
end
%a = @sum(11,@prod(%%mapXbloc,%%mapYbloc))
list seek,1,%a
%a = @succ(%a)
%t = @next(1)
%b = @prod(4,%t)

%a = @succ(%a)
%a = @succ(%a)
list seek,1,@sum(%a,%b)
%t = @next(1)
%u = @next(1)
%t = @prod(%t,256)
%%unitcount = @sum(%t,%u)
if @zero(%%unitcount)
	exit
end

%c = 0
repeat
	%c = @succ(%c)
	%%unittype = @next(1)

	%t = @next(1)
	%u = @next(1)
	%t = @prod(%t,256)
	%t = @sum(%t,%u)
	%%unitX = @div(%t,24)

	%t = @next(1)
	%u = @next(1)
	%t = @prod(%t,256)
	%t = @sum(%t,%u)
	%%unitY = @div(%t,24)

	%%unitcolor = 0
	if @greater(%%unittype,11)
		%%unitcolor = 1
	end
	if @greater(%%unittype,23)
		%%unitcolor = 2
	end
	if @greater(%%unittype,35)
		%%unitcolor = 3
	end

	if @greater(2,@len(%%unittype))
		%%unittype = "0"%%unittype
	end
	dialog set,"b_"%%unitX"x"%%unitY,@path(%0)"AE2map.bin|"@prod(@sum(%%unittype,47),406)
	rem info @path(%0)"textures\unit_icons_"%%unittype".bmp"
	rem info '%%unitcount' '%%unittype' '%%unitX' '%%unitY'
until @equal(%c,%%unitcount)
rem info drawunit todo '%%unitcount'
exit
_


:GRIDCLICK
%%grid = @dlgtext(grid)
%%setposblocks = 1
gosub initscreen
goto evloop


:UNITADD
option fieldsep,"X"
parse "%%unitXs;%%unitYs",%%BLOCEVENT
%a = @sum(11,@prod(%%mapXbloc,%%mapYbloc))
list seek,1,%a
%a = @succ(%a)
%t = @next(1)
%b = @prod(4,%t)

%a = @succ(%a)
%a = @succ(%a)
list seek,1,@sum(%a,%b)
%t = @next(1)
%u = @next(1)
%t = @prod(%t,256)
%%unitcount = @sum(%t,%u)
%%unitstart = @index(1)
%%unitalloc =

if @greater(%%unitcount,0)
	rem search for replace
	%c = 0
	repeat
		%%unittype = @next(1)
	
		%t = @next(1)
		%u = @next(1)
		%t = @prod(%t,256)
		%t = @sum(%t,%u)
		%%unitX = @div(%t,24)
	
		%t = @next(1)
		%u = @next(1)
		%t = @prod(%t,256)
		%t = @sum(%t,%u)
		%%unitY = @div(%t,24)

		if @both(@equal(%%unitX,%%unitXs),@equal(%%unitY,%%unitYs))
			if %%unitalloc
				warn "found 2 unit in the same location ! (loc: "%%unitXs" x "%%unitYs")"@cr()"Try to keep only one!"
			else
				%%unitalloc = %c
			end
		end
		%c = @succ(%c)
	until @equal(%c,%%unitcount)
end

if @not(%%unitalloc)
	list add,1,255
	list add,1,255
	list add,1,255
	list add,1,255
	list add,1,255
	%%unitalloc = %%unitcount
	%%unitcount = @succ(%%unitcount)
end

%%unittype = @sum(@prod(12,%%unitcolorswitch),%%unitsel)
list seek,1,@sum(%%unitstart,@prod(%%unitalloc,5))
rem list insert,1,TAEE
list put,1,@sum(@prod(12,%%unitcolorswitch),%%unitsel)

%i = @sum(12,@prod(%%unitXs,24))
%t = @div(%i,256)
%j = @prod(%t,256)
%u = @diff(%i,%j)

%%VDSDEMERDE = @next(1)
list put,1,%t

%%VDSDEMERDE = @next(1)
list put,1,%u

%i = @sum(12,@prod(%%unitYs,24))
%t = @div(%i,256)
%j = @prod(%t,256)
%u = @diff(%i,%j)

%%VDSDEMERDE = @next(1)
list put,1,%t

%%VDSDEMERDE = @next(1)
list put,1,%u

list seek,1,@diff(%%unitstart,1)

%i = %%unitcount
%t = @div(%i,256)
%j = @prod(%t,256)
%u = @diff(%i,%j)
list put,1,%t
list put,1,%u

dialog set,"b_"%%unitXs"x"%%unitYs,@path(%0)"AE2map.bin|"@prod(@sum(%%unittype,47),406)

exit
_

:UNITDEL
option fieldsep,"X"
parse "%%unitXs;%%unitYs",%%BLOCEVENT
%a = @sum(11,@prod(%%mapXbloc,%%mapYbloc))
list seek,1,%a
%a = @succ(%a)
%t = @next(1)
%b = @prod(4,%t)

%a = @succ(%a)
%a = @succ(%a)
list seek,1,@sum(%a,%b)
%t = @next(1)
%u = @next(1)
%t = @prod(%t,256)
%%unitcount = @sum(%t,%u)
%%unitstart = @index(1)
%%unitalloc =

if @greater(%%unitcount,0)
	rem search for replace
	%c = 0
	repeat
		%%unittype = @next(1)
	
		%t = @next(1)
		%u = @next(1)
		%t = @prod(%t,256)
		%t = @sum(%t,%u)
		%%unitX = @div(%t,24)
	
		%t = @next(1)
		%u = @next(1)
		%t = @prod(%t,256)
		%t = @sum(%t,%u)
		%%unitY = @div(%t,24)

		if @both(@equal(%%unitX,%%unitXs),@equal(%%unitY,%%unitYs))
			if %%unitalloc
				warn "found 2 unit in the same location ! (loc: "%%unitXs" x "%%unitYs")"@cr()"Try to keep only one! Refresh manually !"
			else
				%%unitalloc = %c
			end
		end
		%c = @succ(%c)
	until @equal(%c,%%unitcount)
else
	rem rien à effacer
	exit
end

if @not(%%unitalloc)
	rem aucune unité trouvée à cet emplacement
	exit
end

list seek,1,@sum(%%unitstart,@prod(%%unitalloc,5))
list delete,1
list delete,1
list delete,1
list delete,1
list delete,1

%%unitcount = @pred(%%unitcount)

list seek,1,@diff(%%unitstart,1)
%i = %%unitcount
%t = @div(%i,256)
%j = @prod(%t,256)
%u = @diff(%i,%j)
list put,1,%t
list put,1,%u

list seek,1,@sum(@sum(8,%%unitYs),@prod(%%mapYbloc,%%unitXs))
%v = @next(1)

dialog set,"b_"%%unitXs"x"%%unitYs,@path(%0)"AE2map.bin|"@prod(%v,406)
exit
_


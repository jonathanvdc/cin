
dsc Flame.C/Flame.C.dsproj -repeat-command -time $@
cd cin
xbuild /p:Configuration=Release cin.sln
cd ..

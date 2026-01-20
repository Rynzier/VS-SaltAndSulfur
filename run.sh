current_path=$(pwd)
project_name="SaltAndSulfur"
if [ -z "$1" ]; then
    u_dll="$VINTAGE_STORY/Vintagestory.dll"
else
    u_dll="$VINTAGE_STORY/$1"
fi

dotnet msbuild "$current_path/$project_name/$project_name.csproj"

dotnet "$u_dll" --tracelog --addModPath "$current_path/$project_name/bin/Debug/Mods" --addOrigin "$current_path/$project_name/assets"

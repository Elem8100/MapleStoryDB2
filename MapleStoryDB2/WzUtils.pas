unit WzUtils;

interface

uses
  Windows, SysUtils, StrUtils,  Generics.Collections, WZIMGFile, Global, Tools,
  WZArchive, WZDirectory;

function NoIMG(const Name: string): string; inline;

function GetImgEntry(Path: string; UseGet2: Boolean = False): TWZIMGEntry;

function HasImgEntry(Path: string): Boolean;

function GetEntryPath(Entry: TWZIMGEntry): string;

function GetImgFile(Path: string): TWZIMGFile;

function HasImgFile(Path: string): Boolean;

function GetUOL(Entry: TWZIMGEntry): TWZIMGEntry;



implementation

function SelectWz(Path: string): TWZArchive;
begin
  case Path[2] of
    'a':
      if Path[4] = '2' then
        Result := Map2Wz
      else if Path[4] = '0' then
        Result := Map001Wz
      else
        Result := MapWz;
    'o':
      if Path[1] = 'S' then
        Result := SoundWZ
      else if Path[3] = 'r' then
        Result := MorphWz
      else if Path[4] = '2' then
        Result := Mob2WZ
      else
        Result := MobWZ;
    'p':
      Result := NPCWZ;
    'h':
      Result := CharacterWZ;
    'k':
      if Path[6] = '0' then
        Result := Skill001Wz
      else
        Result := SkillWZ;

    't':
      if Path[1] = 'I' then
        Result := ItemWZ
      else if Path[3] = 'c' then
        Result := EtcWZ
      else
        Result := StringWZ;
  end;
end;

function NoIMG(const Name: string): string; inline;
begin
  Result := ChangeFileExt(Name, '');
end;

function GetUOL(Entry: TWZIMGEntry): TWZIMGEntry;
begin
   Result := TWZIMGEntry(Entry.Parent).Get2(Entry.GetPath);
end;

function GetImgEntry(Path: string; UseGet2: Boolean = False): TWZIMGEntry;
var
  S: TStringArray;
  ImgName: string;
  WZ: TWZArchive;
  Len: Integer;
begin
  WZ := SelectWz(Path);
  S := Explode('.img/', Path);
  Len := Pos('/', S[0]) + 1;
  ImgName := MidStr(S[0], Len, 100) + '.img';
  if UseGet2 then
    Result := WZ.GetImgFile(ImgName).Root.Get2(S[1])
  else
    Result := WZ.GetImgFile(ImgName).Root.Get(S[1]);
end;

function GetImgFile(Path: string): TWZIMGFile;
var
  S: TStringArray;
  ImgName: string;
  WZ: TWZArchive;
  Len: Integer;
begin
  WZ := SelectWz(Path);
  S := Explode('.img/', Path);
  Len := Pos('/', S[0]) + 1;
  ImgName := MidStr(S[0], Len, 100) + '.img';
  Result := WZ.GetImgFile(ImgName);
end;

function HasImgEntry(Path: string): Boolean;
begin
  Result := GetImgEntry(Path) <> nil;
end;

function HasImgFile(Path: string): Boolean;
begin
  Result := GetImgFile(Path + '/') <> nil;
end;


function GetEntryPath(Entry: TWZIMGEntry): string;
var
  Path: string;
  E: TWZEntry;
begin
  Path := Entry.Name;
  E := Entry.Parent;
  while E <> nil do
  begin
    Path := E.Name + '/' + Path;
    E := E.Parent;
  end;
  Result := Path;
end;



end.


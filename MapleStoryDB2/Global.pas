unit Global;

interface

uses
  Windows, SysUtils, StrUtils, WZArchive, Generics.Collections, WZIMGFile, WZDirectory, Classes,
  Math, BassHandler, Tools, System.Types;

var
  MobWZ, Mob2WZ, NPCWZ, MapWz, Map2Wz, Map001Wz, MorphWz, StringWZ, SoundWZ, Sound2Wz, CharacterWZ,
    SkillWZ, Skill001Wz, ItemWZ, EtcWZ: TWZArchive;

function IsNumber(AStr: string): Boolean;

function TrimS(Stemp: string): string;

implementation

function IsNumber(AStr: string): Boolean;
var
  Value: Double;
  Code: Integer;
begin
  Val(AStr, Value, Code);
  Result := Code = 0;
end;

function TrimS(Stemp: string): string;
const
  Remove =['.', '/', #13, #10];
var
  I: Integer;
begin
  Result := '';
  for I := 1 to Length(Stemp) do
  begin
    if not (Stemp[I] in Remove) then
      Result := Result + Stemp[I];
  end;
end;

end.


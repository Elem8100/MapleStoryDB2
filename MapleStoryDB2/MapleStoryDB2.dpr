program MapleStoryDB2;
      {$SetPEFlags $20}
uses
  Forms,
  MainUnit1 in 'D:\XE 1\MainUnit1.pas' {Form1};

{$R *.res}

begin
  Application.Initialize;
  Application.MainFormOnTaskbar := True;
  Application.CreateForm(TForm1, Form1);
  Application.Run;
end.

{*****************************************************************************
*  ZLibEx.pas                                                                *
*                                                                            *
*  copyright (c) 2000-2003 base2 technologies                                *
*  copyright (c) 1997 Borland International                                  *
*                                                                            *
*  revision history                                                          *
*    2005.01.05  Changes by Lifepower (lifepower@mail333.com):               *
*                moved constant section to public                            *
*                added forward declarations to make this unit compatible     *
*                with PNGImage component.                                    *
*    2004.01.06  updated to zlib version 1.2.1                               *
*    2003.04.14  added ZCompress2 and ZDecompress2                           *
*                added ZCompressStr2 and ZDecompressStr2                     *
*                added ZCompressStream2 and ZDecompressStream2               *
*                added overloaded T*Stream constructors to support           *
*                  InflateInit2 and DeflateInit2                             *
*                fixed ZDecompressStream to use ZDecompressCheck instead of  *
*                  ZCompressCheck                                            *
*    2002.03.15  updated to zlib version 1.1.4                               *
*    2001.11.27  enhanced TZDecompressionStream.Read to adjust source        *
*                  stream position upon end of compression data              *
*                fixed endless loop in TZDecompressionStream.Read when       *
*                  destination count was greater than uncompressed data      *
*    2001.10.26  renamed unit to integrate "nicely" with delphi 6            *
*    2000.11.24  added soFromEnd condition to TZDecompressionStream.Seek     *
*                added ZCompressStream and ZDecompressStream                 *
*    2000.06.13  optimized, fixed, rewrote, and enhanced the zlib.pas unit   *
*                  included on the delphi cd (zlib version 1.1.3)            *
*                                                                            *
*  acknowledgements                                                          *
*    erik turner    Z*Stream routines                                        *
*    david bennion  finding the nastly little endless loop quirk with the    *
*                     TZDecompressionStream.Read method                      *
*    burak kalayci  informing me about the zlib 1.1.4 update and the 1.2.1   *
*                     update                                                 *
*****************************************************************************}

unit ZLibEx;

interface

uses
  Sysutils, Classes;

const
  ZLIB_VERSION = '1.2.1';
  ZLIB_VERNUM  = $1210;

type
  TZAlloc = function (opaque: Pointer; items, size: Integer): Pointer;
  TZFree  = procedure (opaque, block: Pointer);

  TZCompressionLevel = (zcNone, zcFastest, zcDefault, zcMax);
  TZStrategy         = (zsDefault, zsFiltered, zsHuffman, zsRLE);

  {** TZStreamRec ***********************************************************}

  TZStreamRec = packed record
    next_in  : PChar;     // next input byte
    avail_in : Longint;   // number of bytes available at next_in
    total_in : Longint;   // total nb of input bytes read so far

    next_out : PChar;     // next output byte should be put here
    avail_out: Longint;   // remaining free space at next_out
    total_out: Longint;   // total nb of bytes output so far

    msg      : PChar;     // last error message, NULL if no error
    state    : Pointer;   // not visible by applications

    zalloc   : TZAlloc;   // used to allocate the internal state
    zfree    : TZFree;    // used to free the internal state
    opaque   : Pointer;   // private data object passed to zalloc and zfree

    data_type: Integer;   // best guess about the data type: ascii or binary
    adler    : Longint;   // adler32 value of the uncompressed data
    reserved : Longint;   // reserved for future use
  end;

  {** TCustomZStream ********************************************************}

  TCustomZStream = class(TStream)
  private
    FStream    : TStream;
    FStreamPos : Integer;
    FOnProgress: TNotifyEvent;

    FZStream   : TZStreamRec;
    FBuffer    : Array [Word] of Char;
  protected
    constructor Create(stream: TStream);

    procedure DoProgress; dynamic;

    property OnProgress: TNotifyEvent read FOnProgress write FOnProgress;
  end;

  {** TZCompressionStream ***************************************************}

  TZCompressionStream = class(TCustomZStream)
  private
    function GetCompressionRate: Single;
  public
    constructor Create(dest: TStream;
      compressionLevel: TZCompressionLevel = zcDefault); overload;

    constructor Create(dest: TStream; compressionLevel: TZCompressionLevel;
      windowBits, memLevel: Integer; strategy: TZStrategy); overload;

    destructor  Destroy; override;

    function  Read(var buffer; count: Longint): Longint; override;
    function  Write(const buffer; count: Longint): Longint; override;
    function  Seek(offset: Longint; origin: Word): Longint; override;

    property CompressionRate: Single read GetCompressionRate;
    property OnProgress;
  end;

  {** TZDecompressionStream *************************************************}

  TZDecompressionStream = class(TCustomZStream)
  public
    constructor Create(source: TStream); overload;
    constructor Create(source: TStream; windowBits: Integer); overload;

    destructor  Destroy; override;

    function  Read(var buffer; count: Longint): Longint; override;
    function  Write(const buffer; count: Longint): Longint; override;
    function  Seek(offset: Longint; origin: Word): Longint; override;

    property OnProgress;
  end;

{** zlib public routines ****************************************************}

{*****************************************************************************
*  ZCompress                                                                 *
*                                                                            *
*  pre-conditions                                                            *
*    inBuffer  = pointer to uncompressed data                                *
*    inSize    = size of inBuffer (bytes)                                    *
*    outBuffer = pointer (unallocated)                                       *
*    level     = compression level                                           *
*                                                                            *
*  post-conditions                                                           *
*    outBuffer = pointer to compressed data (allocated)                      *
*    outSize   = size of outBuffer (bytes)                                   *
*****************************************************************************}

procedure ZCompress(const inBuffer: Pointer; inSize: Integer;
  out outBuffer: Pointer; out outSize: Integer;
  level: TZCompressionLevel = zcDefault);

{*****************************************************************************
*  ZCompress2                                                                *
*                                                                            *
*  pre-conditions                                                            *
*    inBuffer   = pointer to uncompressed data                               *
*    inSize     = size of inBuffer (bytes)                                   *
*    outBuffer  = pointer (unallocated)                                      *
*    level      = compression level                                          *
*    method     = compression method                                         *
*    windowBits = window bits                                                *
*    memLevel   = memory level                                               *
*    strategy   = strategy                                                   *
*                                                                            *
*  post-conditions                                                           *
*    outBuffer = pointer to compressed data (allocated)                      *
*    outSize   = size of outBuffer (bytes)                                   *
*****************************************************************************}

procedure ZCompress2(const inBuffer: Pointer; inSize: Integer;
  out outBuffer: Pointer; out outSize: Integer; level: TZCompressionLevel;
  windowBits, memLevel: Integer; strategy: TZStrategy);

{*****************************************************************************
*  ZDecompress                                                               *
*                                                                            *
*  pre-conditions                                                            *
*    inBuffer    = pointer to compressed data                                *
*    inSize      = size of inBuffer (bytes)                                  *
*    outBuffer   = pointer (unallocated)                                     *
*    outEstimate = estimated size of uncompressed data (bytes)               *
*                                                                            *
*  post-conditions                                                           *
*    outBuffer = pointer to decompressed data (allocated)                    *
*    outSize   = size of outBuffer (bytes)                                   *
*****************************************************************************}

procedure ZDecompress(const inBuffer: Pointer; inSize: Integer;
 out outBuffer: Pointer; out outSize: Integer; outEstimate: Integer = 0);

{*****************************************************************************
*  ZDecompress2                                                              *
*                                                                            *
*  pre-conditions                                                            *
*    inBuffer    = pointer to compressed data                                *
*    inSize      = size of inBuffer (bytes)                                  *
*    outBuffer   = pointer (unallocated)                                     *
*    windowBits  = window bits                                               *
*    outEstimate = estimated size of uncompressed data (bytes)               *
*                                                                            *
*  post-conditions                                                           *
*    outBuffer = pointer to decompressed data (allocated)                    *
*    outSize   = size of outBuffer (bytes)                                   *
*****************************************************************************}

procedure ZDecompress2(const inBuffer: Pointer; inSize: Integer;
 out outBuffer: Pointer; out outSize: Integer; windowBits: Integer;
 outEstimate: Integer = 0);

{** string routines *********************************************************}

function ZCompressStr(const s: String;
  level: TZCompressionLevel = zcDefault): String;

function ZCompressStrEx(const s: String;
  level: TZCompressionLevel = zcDefault): String;

function ZCompressStr2(const s: String; level: TZCompressionLevel;
  windowBits, memLevel: Integer; strategy: TZStrategy): String;

function ZDecompressStr(const s: String): String;

function ZDecompressStrEx(const s: String): String;

function ZDecompressStr2(const s: String; windowBits: Integer): String;

{** stream routines *********************************************************}

procedure ZCompressStream(inStream, outStream: TStream;
  level: TZCompressionLevel = zcDefault);

procedure ZCompressStream2(inStream, outStream: TStream;
  level: TZCompressionLevel; windowBits, memLevel: Integer;
  strategy: TZStrategy);

procedure ZDecompressStream(inStream, outStream: TStream);

procedure ZDecompressStream2(inStream, outStream: TStream;
  windowBits: Integer);


function inflateInit_(var strm: TZStreamRec; version: PChar;
  recsize: Integer): Integer; forward;
function inflate(var strm: TZStreamRec; flush: Integer): Integer; forward;
function inflateEnd(var strm: TZStreamRec): Integer; forward;
function deflateInit_(var strm: TZStreamRec; level: Integer; version: PChar;
  recsize: Integer): Integer; forward;
function deflate(var strm: TZStreamRec; flush: Integer): Integer; forward;
function deflateEnd(var strm: TZStreamRec): Integer; forward;


{****************************************************************************}

type
  EZLibError = class(Exception);

  EZCompressionError = class(EZLibError);
  EZDecompressionError = class(EZLibError);

const
  {** flush constants *******************************************************}
  Z_NO_FLUSH      = 0;
  Z_PARTIAL_FLUSH = 1;
  Z_SYNC_FLUSH    = 2;
  Z_FULL_FLUSH    = 3;
  Z_FINISH        = 4;
  Z_BLOCK         = 5;

  {** return codes **********************************************************}

  Z_OK            = 0;
  Z_STREAM_END    = 1;
  Z_NEED_DICT     = 2;
  Z_ERRNO         = (-1);
  Z_STREAM_ERROR  = (-2);
  Z_DATA_ERROR    = (-3);
  Z_MEM_ERROR     = (-4);
  Z_BUF_ERROR     = (-5);
  Z_VERSION_ERROR = (-6);

  {** compression levels ****************************************************}

  Z_NO_COMPRESSION       =   0;
  Z_BEST_SPEED           =   1;
  Z_BEST_COMPRESSION     =   9;
  Z_DEFAULT_COMPRESSION  = (-1);

  {** compression strategies ************************************************}

  Z_FILTERED            = 1;
  Z_HUFFMAN_ONLY        = 2;
  Z_RLE                 = 3;
  Z_DEFAULT_STRATEGY    = 0;

  {** data types ************************************************************}

  Z_BINARY   = 0;
  Z_ASCII    = 1;
  Z_UNKNOWN  = 2;

  {** compression methods ***************************************************}

  Z_DEFLATED = 8;

  {** return code messages **************************************************}

  _z_errmsg: array[0..9] of PChar = (
    'need dictionary',      // Z_NEED_DICT      (2)
    'stream end',           // Z_STREAM_END     (1)
    '',                     // Z_OK             (0)
    'file error',           // Z_ERRNO          (-1)
    'stream error',         // Z_STREAM_ERROR   (-2)
    'data error',           // Z_DATA_ERROR     (-3)
    'insufficient memory',  // Z_MEM_ERROR      (-4)
    'buffer error',         // Z_BUF_ERROR      (-5)
    'incompatible version', // Z_VERSION_ERROR  (-6)
    ''
  );

  ZLevels: Array [TZCompressionLevel] of Shortint = (
    Z_NO_COMPRESSION,
    Z_BEST_SPEED,
    Z_DEFAULT_COMPRESSION,
    Z_BEST_COMPRESSION
  );

  ZStrategies: Array [TZStrategy] of Shortint = (
    Z_DEFAULT_STRATEGY,
    Z_FILTERED,
    Z_HUFFMAN_ONLY,
    Z_RLE
  );

  SZInvalid = 'Invalid ZStream operation!';

implementation

{** link zlib code **********************************************************}

{$L obj\deflate.obj}
{$L obj\inflate.obj}
{$L obj\inftrees.obj}
{$L obj\infback.obj}
{$L obj\inffast.obj}
{$L obj\trees.obj}
{$L obj\compress.obj}
{$L obj\adler32.obj}
{$L obj\crc32.obj}

{*****************************************************************************
*  note: do not reorder the above -- doing so will result in external        *
*  functions being undefined                                                 *
*****************************************************************************}


{** deflate routines ********************************************************}

function deflateInit_(var strm: TZStreamRec; level: Integer; version: PChar;
  recsize: Integer): Integer;
  external;

function deflateInit2_(var strm: TZStreamRec; level, method, windowBits,
  memLevel, strategy: Integer; version: PChar; recsize: Integer): Integer;
  external;

function deflate(var strm: TZStreamRec; flush: Integer): Integer;
  external;

function deflateEnd(var strm: TZStreamRec): Integer;
  external;

{** inflate routines ********************************************************}

function inflateInit_(var strm: TZStreamRec; version: PChar;
  recsize: Integer): Integer;
  external;

function inflateInit2_(var strm: TZStreamRec; windowBits: Integer;
  version: PChar; recsize: Integer): Integer;
  external;

function inflate(var strm: TZStreamRec; flush: Integer): Integer;
  external;

function inflateEnd(var strm: TZStreamRec): Integer;
  external;

function inflateReset(var strm: TZStreamRec): Integer;
  external;

{** zlib function implementations *******************************************}

function zcalloc(opaque: Pointer; items, size: Integer): Pointer;
begin
  GetMem(result,items * size);
end;

procedure zcfree(opaque, block: Pointer);
begin
  FreeMem(block);
end;

{** c function implementations **********************************************}

procedure _memset(p: Pointer; b: Byte; count: Integer); cdecl;
begin
  FillChar(p^,count,b);
end;

procedure _memcpy(dest, source: Pointer; count: Integer); cdecl;
begin
  Move(source^,dest^,count);
end;

{** custom zlib routines ****************************************************}

function DeflateInit(var stream: TZStreamRec; level: Integer): Integer;
begin
  result := deflateInit_(stream,level,ZLIB_VERSION,SizeOf(TZStreamRec));
end;

function DeflateInit2(var stream: TZStreamRec; level, method, windowBits,
  memLevel, strategy: Integer): Integer;
begin
  result := deflateInit2_(stream,level,method,windowBits,memLevel,strategy,
    ZLIB_VERSION,SizeOf(TZStreamRec));
end;

function InflateInit(var stream: TZStreamRec): Integer;
begin
  result := inflateInit_(stream,ZLIB_VERSION,SizeOf(TZStreamRec));
end;

function InflateInit2(var stream: TZStreamRec; windowBits: Integer): Integer;
begin
  result := inflateInit2_(stream,windowBits,ZLIB_VERSION,SizeOf(TZStreamRec));
end;

{****************************************************************************}

function ZCompressCheck(code: Integer): Integer;
begin
  result := code;

  if code < 0 then
  begin
    raise EZCompressionError.Create(_z_errmsg[2 - code]);
  end;
end;

function ZDecompressCheck(code: Integer): Integer;
begin
  Result := code;

  if code < 0 then
  begin
    raise EZDecompressionError.Create(_z_errmsg[2 - code]);
  end;
end;

procedure ZInternalCompress(var zstream: TZStreamRec; const inBuffer: Pointer;
  inSize: Integer; out outBuffer: Pointer; out outSize: Integer);
const
  delta = 256;
begin
  outSize := ((inSize + (inSize div 10) + 12) + 255) and not 255;
  GetMem(outBuffer,outSize);

  try
    try
      zstream.next_in := inBuffer;
      zstream.avail_in := inSize;
      zstream.next_out := outBuffer;
      zstream.avail_out := outSize;

      while ZCompressCheck(deflate(zstream,Z_FINISH)) <> Z_STREAM_END do
      begin
        Inc(outSize,delta);
        ReallocMem(outBuffer,outSize);

        zstream.next_out := PChar(Integer(outBuffer) + zstream.total_out);
        zstream.avail_out := delta;
      end;
    finally
      ZCompressCheck(deflateEnd(zstream));
    end;

    ReallocMem(outBuffer,zstream.total_out);
    outSize := zstream.total_out;
  except
    FreeMem(outBuffer);
    raise;
  end;
end;

procedure ZInternalDecompress(zstream: TZStreamRec; const inBuffer: Pointer;
  inSize: Integer; out outBuffer: Pointer; out outSize: Integer;
  outEstimate: Integer);
var
  delta: Integer;
begin
  delta := (inSize + 255) and not 255;

  if outEstimate = 0 then outSize := delta
  else outSize := outEstimate;

  GetMem(outBuffer,outSize);

  try
    try
      zstream.next_in := inBuffer;
      zstream.avail_in := inSize;
      zstream.next_out := outBuffer;
      zstream.avail_out := outSize;

      while ZDecompressCheck(inflate(zstream,Z_NO_FLUSH)) <> Z_STREAM_END do
      begin
        Inc(outSize,delta);
        ReallocMem(outBuffer,outSize);

        zstream.next_out := PChar(Integer(outBuffer) + zstream.total_out);
        zstream.avail_out := delta;
      end;
    finally
      ZDecompressCheck(inflateEnd(zstream));
    end;

    ReallocMem(outBuffer,zstream.total_out);
    outSize := zstream.total_out;
  except
    FreeMem(outBuffer);
    raise;
  end;
end;

procedure ZCompress(const inBuffer: Pointer; inSize: Integer;
  out outBuffer: Pointer; out outSize: Integer;
  level: TZCompressionLevel);
var
  zstream: TZStreamRec;
begin
  FillChar(zstream,SizeOf(TZStreamRec),0);

  ZCompressCheck(DeflateInit(zstream,ZLevels[level]));

  ZInternalCompress(zstream,inBuffer,inSize,outBuffer,outSize);
end;

procedure ZCompress2(const inBuffer: Pointer; inSize: Integer;
  out outBuffer: Pointer; out outSize: Integer; level: TZCompressionLevel;
  windowBits, memLevel: Integer; strategy: TZStrategy);
var
  zstream: TZStreamRec;
begin
  FillChar(zstream,SizeOf(TZStreamRec),0);

  ZCompressCheck(DeflateInit2(zstream,ZLevels[level],Z_DEFLATED,windowBits,
    memLevel,ZStrategies[strategy]));

  ZInternalCompress(zstream,inBuffer,inSize,outBuffer,outSize);
end;

procedure ZDecompress(const inBuffer: Pointer; inSize: Integer;
  out outBuffer: Pointer; out outSize: Integer; outEstimate: Integer);
var
  zstream: TZStreamRec;
begin
  FillChar(zstream,SizeOf(TZStreamRec),0);

  ZDecompressCheck(InflateInit(zstream));

  ZInternalDecompress(zstream,inBuffer,inSize,outBuffer,outSize,outEstimate);
end;

procedure ZDecompress2(const inBuffer: Pointer; inSize: Integer;
  out outBuffer: Pointer; out outSize: Integer; windowBits: Integer;
  outEstimate: Integer);
var
  zstream: TZStreamRec;
begin
  FillChar(zstream,SizeOf(TZStreamRec),0);

  ZDecompressCheck(InflateInit2(zstream,windowBits));

  ZInternalDecompress(zstream,inBuffer,inSize,outBuffer,outSize,outEstimate);
end;

{** string routines *********************************************************}

function ZCompressStr(const s: String; level: TZCompressionLevel): String;
var
  buffer: Pointer;
  size  : Integer;
begin
  ZCompress(PChar(s),Length(s),buffer,size,level);

  SetLength(result,size);
  Move(buffer^,result[1],size);

  FreeMem(buffer);
end;

function ZCompressStrEx(const s: String; level: TZCompressionLevel): String;
var
  buffer: Pointer;
  size  : Integer;
begin
  ZCompress(PChar(s),Length(s),buffer,size,level);

  SetLength(result,size + SizeOf(Integer));

  Move(buffer^,result[5],size);

  size := Length(s);
  Move(size,result[1],SizeOf(Integer));

  FreeMem(buffer);
end;

function ZCompressStr2(const s: String; level: TZCompressionLevel;
  windowBits, memLevel: Integer; strategy: TZStrategy): String;
var
  buffer: Pointer;
  size  : Integer;
begin
  ZCompress2(PChar(s),Length(s),buffer,size,level,windowBits,memLevel,
    strategy);

  SetLength(result,size);
  Move(buffer^,result[1],size);

  FreeMem(buffer);
end;

function ZDecompressStr(const s: String): String;
var
  buffer: Pointer;
  size  : Integer;
begin
  ZDecompress(PChar(s),Length(s),buffer,size);

  SetLength(result,size);
  Move(buffer^,result[1],size);

  FreeMem(buffer);
end;

function ZDecompressStrEx(const s: String): String;
var
  buffer  : Pointer;
  size    : Integer;
  data    : String;
  dataSize: Integer;
begin
  Move(s[1],size,SizeOf(Integer));

  dataSize := Length(s) - SizeOf(Integer);

  SetLength(data,dataSize);
  Move(s[5],data[1],dataSize);

  ZDecompress(PChar(data),dataSize,buffer,size,size);

  SetLength(result,size);
  Move(buffer^,result[1],size);

  FreeMem(buffer);
end;

function ZDecompressStr2(const s: String; windowBits: Integer): String;
var
  buffer: Pointer;
  size  : Integer;
begin
  ZDecompress2(PChar(s),Length(s),buffer,size,windowBits);

  SetLength(result,size);
  Move(buffer^,result[1],size);

  FreeMem(buffer);
end;

{** stream routines *********************************************************}

procedure ZInternalCompressStream(zstream: TZStreamRec; inStream,
  outStream: TStream);
const
  bufferSize = 32768;
var
  zresult  : Integer;
  inBuffer : Array [0..bufferSize-1] of Char;
  outBuffer: Array [0..bufferSize-1] of Char;
  inSize   : Integer;
  outSize  : Integer;
begin
  inSize := inStream.Read(inBuffer,bufferSize);

  while inSize > 0 do
  begin
    zstream.next_in := inBuffer;
    zstream.avail_in := inSize;

    repeat
      zstream.next_out := outBuffer;
      zstream.avail_out := bufferSize;

      ZCompressCheck(deflate(zstream,Z_NO_FLUSH));

      // outSize := zstream.next_out - outBuffer;
      outSize := bufferSize - zstream.avail_out;

      outStream.Write(outBuffer,outSize);
    until (zstream.avail_in = 0) and (zstream.avail_out > 0);

    inSize := inStream.Read(inBuffer,bufferSize);
  end;

  repeat
    zstream.next_out := outBuffer;
    zstream.avail_out := bufferSize;

    zresult := ZCompressCheck(deflate(zstream,Z_FINISH));

    // outSize := zstream.next_out - outBuffer;
    outSize := bufferSize - zstream.avail_out;

    outStream.Write(outBuffer,outSize);
  until (zresult = Z_STREAM_END) and (zstream.avail_out > 0);

  ZCompressCheck(deflateEnd(zstream));
end;

procedure ZInternalDecompressStream(zstream: TZStreamRec; inStream,
  outStream: TStream);
const
  bufferSize = 32768;
var
  zresult  : Integer;
  inBuffer : Array [0..bufferSize-1] of Char;
  outBuffer: Array [0..bufferSize-1] of Char;
  inSize   : Integer;
  outSize  : Integer;
begin
  inSize := inStream.Read(inBuffer,bufferSize);

  while inSize > 0 do
  begin
    zstream.next_in := inBuffer;
    zstream.avail_in := inSize;

    repeat
      zstream.next_out := outBuffer;
      zstream.avail_out := bufferSize;

      ZDecompressCheck(inflate(zstream,Z_NO_FLUSH));

      // outSize := zstream.next_out - outBuffer;
      outSize := bufferSize - zstream.avail_out;

      outStream.Write(outBuffer,outSize);
    until (zstream.avail_in = 0) and (zstream.avail_out > 0);

    inSize := inStream.Read(inBuffer,bufferSize);
  end;

  repeat
    zstream.next_out := outBuffer;
    zstream.avail_out := bufferSize;

    zresult := ZDecompressCheck(inflate(zstream,Z_FINISH));

    // outSize := zstream.next_out - outBuffer;
    outSize := bufferSize - zstream.avail_out;

    outStream.Write(outBuffer,outSize);
  until (zresult = Z_STREAM_END) and (zstream.avail_out > 0);

  ZDecompressCheck(inflateEnd(zstream));
end;

procedure ZCompressStream(inStream, outStream: TStream;
  level: TZCompressionLevel);
var
  zstream: TZStreamRec;
begin
  FillChar(zstream,SizeOf(TZStreamRec),0);

  ZCompressCheck(DeflateInit(zstream,ZLevels[level]));

  ZInternalCompressStream(zstream,inStream,outStream);
end;

procedure ZCompressStream2(inStream, outStream: TStream;
  level: TZCompressionLevel; windowBits, memLevel: Integer;
  strategy: TZStrategy);
var
  zstream: TZStreamRec;
begin
  FillChar(zstream,SizeOf(TZStreamRec),0);

  ZCompressCheck(DeflateInit2(zstream,ZLevels[level],Z_DEFLATED,windowBits,
    memLevel,ZStrategies[strategy]));

  ZInternalCompressStream(zstream,inStream,outStream);
end;

procedure ZDecompressStream(inStream, outStream: TStream);
var
  zstream: TZStreamRec;
begin
  FillChar(zstream,SizeOf(TZStreamRec),0);

  ZDecompressCheck(InflateInit(zstream));

  ZInternalDecompressStream(zstream,inStream,outStream);
end;

procedure ZDecompressStream2(inStream, outStream: TStream;
  windowBits: Integer);
var
  zstream: TZStreamRec;
begin
  FillChar(zstream,SizeOf(TZStreamRec),0);

  ZDecompressCheck(InflateInit2(zstream,windowBits));

  ZInternalDecompressStream(zstream,inStream,outStream);
end;

{** TCustomZStream **********************************************************}

constructor TCustomZStream.Create(stream: TStream);
begin
  inherited Create;

  FStream := stream;
  FStreamPos := stream.Position;
end;

procedure TCustomZStream.DoProgress;
begin
  if Assigned(FOnProgress) then FOnProgress(Self);
end;

{** TZCompressionStream *****************************************************}

constructor TZCompressionStream.Create(dest: TStream;
  compressionLevel: TZCompressionLevel);
begin
  inherited Create(dest);

  FZStream.next_out := FBuffer;
  FZStream.avail_out := SizeOf(FBuffer);

  ZCompressCheck(DeflateInit(FZStream,ZLevels[compressionLevel]));
end;

constructor TZCompressionStream.Create(dest: TStream;
  compressionLevel: TZCompressionLevel; windowBits, memLevel: Integer;
  strategy: TZStrategy);
begin
  inherited Create(dest);

  FZStream.next_out := FBuffer;
  FZStream.avail_out := SizeOf(FBuffer);

  ZCompressCheck(DeflateInit2(FZStream,ZLevels[compressionLevel],Z_DEFLATED,
    windowBits,memLevel,ZStrategies[strategy]));
end;

destructor TZCompressionStream.Destroy;
begin
  FZStream.next_in := Nil;
  FZStream.avail_in := 0;

  try
    if FStream.Position <> FStreamPos then FStream.Position := FStreamPos;

    while ZCompressCheck(deflate(FZStream,Z_FINISH)) <> Z_STREAM_END do
    begin
      FStream.WriteBuffer(FBuffer,SizeOf(FBuffer) - FZStream.avail_out);

      FZStream.next_out := FBuffer;
      FZStream.avail_out := SizeOf(FBuffer);
    end;

    if FZStream.avail_out < SizeOf(FBuffer) then
    begin
      FStream.WriteBuffer(FBuffer,SizeOf(FBuffer) - FZStream.avail_out);
    end;
  finally
    deflateEnd(FZStream);
  end;

  inherited Destroy;
end;

function TZCompressionStream.Read(var buffer; count: Longint): Longint;
begin
  raise EZCompressionError.Create(SZInvalid);
end;

function TZCompressionStream.Write(const buffer; count: Longint): Longint;
begin
  FZStream.next_in := @buffer;
  FZStream.avail_in := count;

  if FStream.Position <> FStreamPos then FStream.Position := FStreamPos;

  while FZStream.avail_in > 0 do
  begin
    ZCompressCheck(deflate(FZStream,Z_NO_FLUSH));

    if FZStream.avail_out = 0 then
    begin
      FStream.WriteBuffer(FBuffer,SizeOf(FBuffer));

      FZStream.next_out := FBuffer;
      FZStream.avail_out := SizeOf(FBuffer);

      FStreamPos := FStream.Position;

      DoProgress;
    end;
  end;

  result := Count;
end;

function TZCompressionStream.Seek(offset: Longint; origin: Word): Longint;
begin
  if (offset = 0) and (origin = soFromCurrent) then
  begin
    result := FZStream.total_in;
  end
  else raise EZCompressionError.Create(SZInvalid);
end;

function TZCompressionStream.GetCompressionRate: Single;
begin
  if FZStream.total_in = 0 then result := 0
  else result := (1.0 - (FZStream.total_out / FZStream.total_in)) * 100.0;
end;

{** TZDecompressionStream ***************************************************}

constructor TZDecompressionStream.Create(source: TStream);
begin
  inherited Create(source);

  FZStream.next_in := FBuffer;
  FZStream.avail_in := 0;

  ZDecompressCheck(InflateInit(FZStream));
end;

constructor TZDecompressionStream.Create(source: TStream;
  windowBits: Integer);
begin
  inherited Create(source);

  FZStream.next_in := FBuffer;
  FZStream.avail_in := 0;

  ZDecompressCheck(InflateInit2(FZStream,windowBits));
end;

destructor TZDecompressionStream.Destroy;
begin
  inflateEnd(FZStream);

  inherited Destroy;
end;

function TZDecompressionStream.Read(var buffer; count: Longint): Longint;
var
  zresult: Integer;
begin
  FZStream.next_out := @buffer;
  FZStream.avail_out := count;

  if FStream.Position <> FStreamPos then FStream.Position := FStreamPos;

  zresult := Z_OK;

  while (FZStream.avail_out > 0) and (zresult <> Z_STREAM_END) do
  begin
    if FZStream.avail_in = 0 then
    begin
      FZStream.avail_in := FStream.Read(FBuffer,SizeOf(FBuffer));

      if FZStream.avail_in = 0 then
      begin
        result := count - FZStream.avail_out;

        Exit;
      end;

      FZStream.next_in := FBuffer;
      FStreamPos := FStream.Position;

      DoProgress;
    end;

    zresult := ZDecompressCheck(inflate(FZStream,Z_NO_FLUSH));
  end;

  if (zresult = Z_STREAM_END) and (FZStream.avail_in > 0) then
  begin
    FStream.Position := FStream.Position - FZStream.avail_in;
    FStreamPos := FStream.Position;

    FZStream.avail_in := 0;
  end;

  result := count - FZStream.avail_out;
end;

function TZDecompressionStream.Write(const Buffer; Count: Longint): Longint;
begin
  raise EZDecompressionError.Create(SZInvalid);
end;

function TZDecompressionStream.Seek(Offset: Longint; Origin: Word): Longint;
var
  buf: Array [0..8191] of Char;
  i  : Integer;
begin
  if (offset = 0) and (origin = soFromBeginning) then
  begin
    ZDecompressCheck(inflateReset(FZStream));

    FZStream.next_in := FBuffer;
    FZStream.avail_in := 0;

    FStream.Position := 0;
    FStreamPos := 0;
  end
  else if ((offset >= 0) and (origin = soFromCurrent)) or
          (((offset - FZStream.total_out) > 0) and (origin = soFromBeginning)) then
  begin
    if origin = soFromBeginning then Dec(offset,FZStream.total_out);

    if offset > 0 then
    begin
      for i := 1 to offset div SizeOf(buf) do ReadBuffer(buf,SizeOf(buf));
      ReadBuffer(buf,offset mod SizeOf(buf));
    end;
  end
  else if (offset = 0) and (origin = soFromEnd) then
  begin
    while Read(buf,SizeOf(buf)) > 0 do ;
  end
  else raise EZDecompressionError.Create(SZInvalid);

  result := FZStream.total_out;
end;

end.

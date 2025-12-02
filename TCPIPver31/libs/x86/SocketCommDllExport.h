#ifndef STATUS_STRUCT
// Note: <long> is a 32-bit integer (for both 32-bit and 64-bit systems)
//		 <int> is 32-bit (for both 32-bit and 64-bit systems)
//
//
#define STATUS_STRUCT
typedef struct{
	unsigned int	d_counter;//actual counter of ok-prints
	unsigned int	s_counter;//actual counter of nok-prints
	unsigned int	n_messageport;//actual external message selection
	unsigned char	Start;//1: system is printing 0: system is stopped
	unsigned char	request;//(internal variable)
	unsigned char	option;//(internal variable)
	unsigned char	res;//reserved (0x0:default, 0x1: UMT enabled, 0x4: Batchjob enabled)
	unsigned int	t_counter;//total counter of prints
	unsigned int	m_copies;//actual nr of copies to be printed
	unsigned int err;//alarmcode (upper WORD: codifies lastactive alarm; lower WORD: 0: noalarm, else: alarm active)
	unsigned int time;//reserved D-Word (actually used as timeofprint-info)
	char  name[8];//actual active filename (max.8 chars)
	unsigned int reserved1;	//reserved
	unsigned int reserved2;	//reserved
} PStatus;
typedef struct{
	unsigned int	d_counter;//actual counter of ok-prints (reseted when message changes)
	unsigned int	s_counter;//actual counter of nok-prints (reseted when message changes)
	unsigned int	n_messageport;//actual file-nr printing
	unsigned char	Start;//printing or stopped
						  //BIT0: printing loop (prepared for printing) BIT1: printing (we are actually marking)
	unsigned char	request;//waiting for request
	unsigned char	option;//options for HandleRequest()
	unsigned char	res;//reserved (0x0:default, 0x1: UMT enabled, 0x4: Batchjob enabled)
	unsigned int	t_counter;//total counter of prints
	unsigned int	m_copies;//actual nr of copies to be print
	unsigned int err;//alarmcode
	unsigned int time;//reserved D-Word (actually used as time-info
	unsigned int alarmmask1;	//reserved
	unsigned int signalstate;	//reserved
	char  messagename[16];//filename (max.16 chars = 12 + 4)
	char  eventhandler[16];//filename (max.16 chars = 12 + 4)
	unsigned int alarmmask2;	//codes extended alarms
	unsigned int extra;//Bit0..Bit9l dynamic usage of scanfield in permille; 
				 // Bit10..Bit13 actual dynamic mode
} PStatusExt;
typedef struct {
	unsigned int size;//in bytes total space
	unsigned int avail;//in bytes available space
} PDrive;
#define SYSINFO_DRIVENR 4
typedef struct {
	unsigned int cputemp;//cpu temperature
	PDrive drive[SYSINFO_DRIVENR];//harddisk,ramdisk,ramfont,log-drive
	float hours;
	__int64 longcounter;
} PSysinfo;

typedef struct {
	unsigned int cputemp;//cpu temperature
	unsigned int boardtemp;//board temperature
	unsigned int humidity;//humidity
	unsigned int voltage1;//5 V
	unsigned int voltage2;//3.3V
	unsigned int fanlocaltemp;
	unsigned int fancurrentpwm;
	unsigned int fantacho;//cntspersec
	unsigned int fanremotetemp;
} PCoretemp;

#endif
#ifndef SOCKETCOMMDLLEXPORT_H
#define SOCKETCOMMDLLEXPORT_H

#define COPY_TO_RAMDISK  0
#define COPY_TO_HARDDISK 1
#define COPY_FROM_HARDDISK 2
#define COPY_FROM_RAMDISK 4

#define COPY_TECHNICIAN 128

#ifndef SYSINFO_DRIVENR

#define SYSINFO_DRIVENR 4

typedef struct {
	unsigned int size;//in bytes total space
	unsigned int avail;//in bytes available space
} PrintDrive;

typedef struct {
	unsigned int cputemp;//cpu temperature in 1/1000 Celsius
	PrintDrive drive[SYSINFO_DRIVENR];//harddisk,ramdisk,ramfont,log-drive
	Float32 hours;//in hours
	Uint64 longcounter;
} PrintSysinfo;

typedef struct {
	unsigned int cputemp;//cpu temperature in 1/1000 Celsius
	unsigned int boardtemp;//board temperature  in 1/1000 Celsius
	unsigned int humidity;//humidity  in 1/1000 percent
	unsigned int voltage1;//5 V  in 1/1000 Volt
	unsigned int voltage2;//3.3V in 1/1000 Volt
	unsigned int fanlocaltemp;//in 1/10 Celsius
	unsigned int fancurrentpwm;//in permille
	unsigned int fantacho;//in cps
	unsigned int fanremotetemp;//in 1/10 Celsius
} PrintCoretemp;

#endif

extern "C" {
	//////////////////////////////////////////////////////////
	//							//
	//General functions to initialize or close a connection	//
	//							//
	//////////////////////////////////////////////////////////
__declspec(dllexport) int __stdcall MGetDllVersion();
__declspec(dllexport) void __stdcall MInit(int &p, wchar_t *name, wchar_t *ip, wchar_t *path); //Initialize socket
__declspec(dllexport) void __stdcall MFinish(int p); //Close socket
__declspec(dllexport) int __stdcall MStartClient(int p); //Establishes connection to the system
__declspec(dllexport) int __stdcall MLaser_Knockout(int p); //should be called before Shutdown client
__declspec(dllexport) int __stdcall MLaser_ServerShutdown(int p, int bexit); //restarts the laser's server(bexit=0) or shuts it down and closes laser's program (bexit=1)
			//to indicate the laser that the connection will be going down
__declspec(dllexport) int __stdcall MShutdownClient(int p); //Close connection to the system
__declspec(dllexport) int __stdcall MGetLastError(int p, wchar_t &txt); //returns pointer to internal char-array; size: 256
__declspec(dllexport) int __stdcall MIsConnected(int p); 
			//returns 0 if system is not connected
			//returns 1 if system is connnected
__declspec(dllexport) int __stdcall MSetTimeout(int p, int timeout);
	//////////////////////////////////////////////////////////////////
	//								//
	//Laser control functions (can be used after initialization)	//	
	//								//
	//////////////////////////////////////////////////////////////////

__declspec(dllexport) int __stdcall MLaser_Stop(int p, int timeout); //Stop signal
			//timeout: in milliseconds, time to wait for answer
__declspec(dllexport) int __stdcall MLaser_FastUsermessage(int p, unsigned char field, const char *text);
			//send string "text" (max. 2040 chars) for <field> to the laser
__declspec(dllexport) int __stdcall MLaser_FastUTF8Usermessage(int p, unsigned char field, const char *text);
			//send string "text" (max. 2040 chars) for <field> to the laser
__declspec(dllexport) int __stdcall MLaser_GetFastUsermessage(int p, unsigned char field, char *buf, int &len);
			//send string "text" (max. 2040 chars) for <field> to the laser
__declspec(dllexport) int __stdcall MLaser_GetFastUTF8Usermessage(int p, unsigned char field, char *buf, int &len);
__declspec(dllexport) int __stdcall MLaser_EnableBufferedUM(int p, int get,int &actsize,int defsize=10);
__declspec(dllexport) int __stdcall MLaser_EnableBufferedUMExt(int p, int get,int &actsize,int &field, int &fillstatus,int defsize);

__declspec(dllexport) int __stdcall MLaser_MultipleUsermessage(int p, char *in, int inlen,char *out, int &outlen, int &fields);
__declspec(dllexport) int __stdcall MLaser_GetMultipleUsermessage(int p, char *in, int inlen,char *out, int &outlen);
__declspec(dllexport) int __stdcall MLaser_MultipleUTF8Usermessage(int p, char *in, int inlen,char *out, int &outlen, int &fields);
__declspec(dllexport) int __stdcall MLaser_GetMultipleUTF8Usermessage(int p, char *in, int inlen,char *out, int &outlen);
__declspec(dllexport) int __stdcall MLaser_GetFifofield(int p, int field, int index,char *out, int &outlen, int &elements);
__declspec(dllexport) int __stdcall MLaser_GetUTF8Fifofield(int p, int field, int index,char *out, int &outlen, int &elements);
__declspec(dllexport) int __stdcall MLaser_FifoDump(int p);


__declspec(dllexport) int __stdcall MLaser_FastDataString(int p, unsigned char field, const char *in, int len);
__declspec(dllexport) int __stdcall MLaser_GetFastDataString(int p, unsigned char field, char *out, int &len);
__declspec(dllexport) int __stdcall MLaser_EnableBufferedDataString(int p, int get,int &actsize,int &field, int &fillstatus,int defsize);

__declspec(dllexport) int __stdcall MLaser_Status(int p, PStatus &status); //gets status of lasersystem
			//fills  status variable with actual status
__declspec(dllexport) int __stdcall MLaser_StatusExt(int p, PStatusExt &statusext); //gets status of lasersystem
			//fills  status variable with actual status
__declspec(dllexport) unsigned short __stdcall MGetVersion(int p); //sets connection timeout variable
			//get version number
__declspec(dllexport) int __stdcall MLaser_GetConnectionData(int p, unsigned char &leading, unsigned char &control, unsigned short &foundmask);
		   //get initial connection data to identify scancard
__declspec(dllexport) int __stdcall MLaser_GetVersionString(int p, char *out, int inlen, int option=0);
			//get the version string
__declspec(dllexport) int __stdcall MLaser_Start(int p, const wchar_t *filename,int nr); //Start signal
			//filename: name of the message inside the laser to be printed (without extension)
			//nr:  number of prints to be done (nr=0 eternal printing, nr=1 testprint)
__declspec(dllexport) int __stdcall MLaser_StartExtended(int p, int nr, int msg,int batch); //Start signal for ext table (batchjob)
			//filename: name of the message inside the laser to be printed (without extension)
			//nr:  number of prints to be done (nr=0 eternal printing, nr=1 testprint)
__declspec(dllexport) int __stdcall MLaser_Reload(int p); //reloads actual printing file
			//used e.g after having sent new data to the laser to reload the actual message
__declspec(dllexport) int __stdcall MLaser_TriggerPrint(int p); //triggers a print (software trigger)
__declspec(dllexport) int __stdcall MLaser_Settime(int p); //Sets actual system time in remote lasersystem
			//uses the OS system time as a reference time
__declspec(dllexport) int __stdcall MLaser_Delete(int p, const wchar_t *name); //deletes file
			//name: filename with extension inside the laser
__declspec(dllexport) int __stdcall MLaser_SetDefault(int p, const wchar_t *name); //sets actual file
			//changes actual message inside the laser to <name> (without extension)
__declspec(dllexport) int __stdcall MLaser_CopyFile(int p, const wchar_t * filename, const wchar_t * path, unsigned char option);
			//sourcefile: local filename to be sent to the laser (msf-file)
			//path: directory ending with "\\" of location of the file 
			//option: copy to ramdisk or to harddisk
__declspec(dllexport) int __stdcall MLaser_PrintMode(int p, unsigned int &mode); 
			//sets and gets printmode (default,UMT,Batch)
			//mode: 0  default mode 1: enable UMT 2: just get actual mode 4: enable batchjob

__declspec(dllexport) int __stdcall MLaser_Mode(int p, unsigned char &mode); //sets mode (static/dynamic)
			//mode: 0  static, 1: dynamic standard 2: dynamic distance
			// 3: dynamic-static
			// 8: fills <mode> variable with actual mode
__declspec(dllexport) int __stdcall MLaser_SetDynamic(int p, int var, int &value); 

__declspec(dllexport) int __stdcall MLaser_GetDynamic(int p, int var, int &value); 


__declspec(dllexport) int __stdcall MLaser_CounterReset(int p); //Reset of d_counter and s_counter (status variable)

__declspec(dllexport) int __stdcall MLaser_SetGlobalCounter(int p, unsigned char field, const char* counter); 
//Set global counter
__declspec(dllexport) int __stdcall MLaser_GetGlobalCounter(int p, unsigned char field, char *counter, int len); 
//Get global counter
__declspec(dllexport) int __stdcall MLaser_SetPrivateCounter(int p, unsigned char field, int repeats, int prints); 
//Set global counter
__declspec(dllexport) int __stdcall MLaser_Offset(int p, int &dx, int &dy, int relative,int format=0,int reset=0); 
//set offset  (format: 0 (ideal coordinates; 1: microns; 2: 0.1mm)

__declspec(dllexport) int __stdcall MLaser_Defocus(int p, int &dz, int relative, int format = 0);

__declspec(dllexport) int __stdcall MLaser_ShiftRotate(int p, float dx, float dy,float deg,float x0, float y0,const wchar_t *player=NULL,const wchar_t *pobj=NULL); 

__declspec(dllexport) int __stdcall MLaser_Powerscale(int p, int set, int member, int &value); 

__declspec(dllexport) int __stdcall MLaser_Setusertime(int p, int day, int month, int year, int hour, int minute, int second);

__declspec(dllexport) int __stdcall MLaser_AsciiConfig(int p, const wchar_t *name,int partial=0);

__declspec(dllexport) int __stdcall MLaser_StartPrintSession(int p, int ignorealarms);

__declspec(dllexport) int __stdcall MLaser_EndPrintSession(int p);

__declspec(dllexport) int __stdcall MLaser_TestPointer(int p,int on);

__declspec(dllexport) int __stdcall MLaser_Eventhandler(int p, const wchar_t *name);

__declspec(dllexport) int __stdcall MLaser_GetFilenames(int p, const wchar_t *extension,int frame,wchar_t *buf, int &bufsize);

__declspec(dllexport) int __stdcall MLaser_Store(int p, int &flags);

__declspec(dllexport) int __stdcall MLaser_MTable(int p);

__declspec(dllexport) int __stdcall MLaser_DumpSVG(int p, const wchar_t *name);

__declspec(dllexport) int __stdcall MLaser_DumpSVGExt(int p, const wchar_t *name, int bfilter, const wchar_t *layername);

__declspec(dllexport) int __stdcall MLaser_Coretemp(int p, PCoretemp &info); 

__declspec(dllexport) int __stdcall MLaser_Sysinfo(int p, PSysinfo &info);

__declspec(dllexport) int __stdcall MLaser_Signalstate(int p, int get, unsigned int &signalstate, unsigned int &enabled);

}
#endif

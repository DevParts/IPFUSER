// SocketCommCPPTestDlg.cpp : implementation file
//

#include "stdafx.h"
#include <winsock2.h>
#include "SocketCommCPPTest.h"
#include "SocketCommCPPTestDlg.h"

#include "..\libs\x86\SocketCommDllExport.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

#define TIMER_ID 100
//just some globals
int sock = 0;
int m_field = 0;
int m_cnts = 0;
unsigned int m_lastcnts = 0;
int m_minfillstatus = 0;
DWORD m_ticstimer = 0;
int m_filllimit = 50;
HANDLE m_hThread=NULL;

struct PrintthreadData{
	CString txt;//input
	int field;//input
	int wait1;//input
	int wait2;//input
	int wait3;//input
	int bdatastring;//input
	int bend;//input
	int limit;//input
	int sock;//input
	unsigned int framecnt;//output
	unsigned int maxtics;//output
};

PrintthreadData g_threaddata;
//Function executed by the server
// thread just reads possible commands and pass them to the datastructure
DWORD WINAPI  Printthread(LPVOID lpParam){
	int prevent = 0;int i; int fillstatus = 0;
	int k = 0;int len; int full = 0; int actsize = 0;
	DWORD t1,t2;struct timeval tv;LARGE_INTEGER pfr;
	DWORD cntsperms;
	char usermess[256];
	char datastring[1024]; DWORD waitms;
	memset(usermess,0,256);
	g_threaddata.framecnt = 0;
	QueryPerformanceFrequency(&pfr);
	cntsperms = (pfr.QuadPart/1000);
	t1 = GetTickCount();
	do{
		if(g_threaddata.sock <=0) break;
		if(!prevent){
			if(g_threaddata.bdatastring){
				if(k>9) k = 0;
				len = (k + 1) * 10;
				memset(datastring,0xFF,1024);//all white
				memset(datastring,0,len);//len bytes black
				len = 1024;
				i = MLaser_FastDataString(g_threaddata.sock,g_threaddata.field,datastring,len);
			}else{
				if(k%2){
					sprintf(usermess,"%s%010u",g_threaddata.txt,k);
				}else{
					sprintf(usermess,"ABC%s%010u",g_threaddata.txt,k);
				}
				i = MLaser_FastUsermessage(g_threaddata.sock,g_threaddata.field,usermess);
			}
		}else{
			i = 0;
		}
		if(i==8){
			full = TRUE;
			if(m_minfillstatus == -1) m_minfillstatus = actsize;//init minimum
		}else if(i){
			break;
		}
		else{
			k++;
			g_threaddata.framecnt++;
			t2 = GetTickCount();
			if(t2 > t1){
				if(t2 - t1 > g_threaddata.maxtics){
					g_threaddata.maxtics = t2 - t1;
				}
			}
			t1 = t2;
		}
		if(full){
			prevent = 1;
		}
		if(k > 9999999999) k = 0;
		fillstatus = 0;
		if(g_threaddata.bdatastring){
			//get fill status
			i = MLaser_EnableBufferedDataString(g_threaddata.sock,1,actsize,g_threaddata.field,fillstatus,0);
		}else{
			//get actual fillstatus
			i = MLaser_EnableBufferedUMExt(g_threaddata.sock, 1,actsize,g_threaddata.field, fillstatus,0);
		}
		if(m_minfillstatus != -1){
			if(fillstatus < m_minfillstatus) m_minfillstatus = fillstatus;
		}
		//enable sending
		if(fillstatus < g_threaddata.limit){
			prevent = 0;
			full = 0;
		}
		if(full) waitms = g_threaddata.wait1;
		else if(prevent) waitms = g_threaddata.wait2;
		else waitms = g_threaddata.wait3;
		//now sleep the time we need
		LARGE_INTEGER pc1,pc2,pdiff;
		DWORD dc,ms;
		QueryPerformanceCounter(&pc1);
		do{
			Sleep(0);
			QueryPerformanceCounter(&pc2);
			//counts passed
			dc = (pc2.QuadPart-pc1.QuadPart);
			//pf is the counts/sec
			ms = dc/cntsperms;
			
		}while(ms < waitms);


	}while(!g_threaddata.bend);//exitcode for thread

	ExitThread(0);
	return 0;
}

//starts a server thread to look for commands
void startprintthread(void){
	m_hThread = ::CreateThread(NULL,8*1024,Printthread,NULL,0,NULL);
}




/////////////////////////////////////////////////////////////////////////////
// CAboutDlg dialog used for App About

class CAboutDlg : public CDialog
{
public:
	CAboutDlg();

// Dialog Data
	//{{AFX_DATA(CAboutDlg)
	enum { IDD = IDD_ABOUTBOX };
	//}}AFX_DATA

	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CAboutDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	//{{AFX_MSG(CAboutDlg)
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

CAboutDlg::CAboutDlg() : CDialog(CAboutDlg::IDD)
{
	//{{AFX_DATA_INIT(CAboutDlg)
	//}}AFX_DATA_INIT
}

void CAboutDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CAboutDlg)
	//}}AFX_DATA_MAP
}

BEGIN_MESSAGE_MAP(CAboutDlg, CDialog)
	//{{AFX_MSG_MAP(CAboutDlg)
		// No message handlers
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CSocketCommCPPTestDlg dialog

CSocketCommCPPTestDlg::CSocketCommCPPTestDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CSocketCommCPPTestDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CSocketCommCPPTestDlg)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
	// Note that LoadIcon does not require a subsequent DestroyIcon in Win32
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
}

void CSocketCommCPPTestDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CSocketCommCPPTestDlg)
	DDX_Control(pDX, IDC_CHECK_DATASTRING, m_cdatastring);
	DDX_Control(pDX, IDC_ENCODERSPEED, m_cencoderspeed);
	DDX_Control(pDX, IDC_PERCENTAGE, m_cpercentage);
	DDX_Control(pDX, IDC_TEST_POWERSCALE3, m_cscale3);
	DDX_Control(pDX, IDC_TEST_POWERSCALE2, m_cscale2);
	DDX_Control(pDX, IDC_TEST_POWERSCALE1, m_cscale1);
	DDX_Control(pDX, IDC_TEST_POWERSCALE0, m_cscale0);
	DDX_Control(pDX, IDC_RESETAFTERPRINT, m_czerooffset);
	DDX_Control(pDX, IDC_TEST_BUFFERSIZE, m_cbuffersize);
	DDX_Control(pDX, IDC_TESTSTOP, m_cteststop);
	DDX_Control(pDX, IDC_TEST_BUFFEREDUM, m_ctestbufferedum);
	DDX_Control(pDX, IDC_IPADDRESS, m_cipaddress);
	DDX_Control(pDX, IDC_TEST_LASER2, m_ctest2);
	DDX_Control(pDX, IDC_TEST_OFFSETY, m_coffsety);
	DDX_Control(pDX, IDC_TEST_OFFSETX, m_coffsetx);
	DDX_Control(pDX, IDC_TEST_LASER, m_ctest);
	DDX_Control(pDX, IDC_TEST_DISCONNECT, m_cdisconnect);
	DDX_Control(pDX, IDC_TEST_CONNECT, m_cconnect);
	DDX_Control(pDX, IDC_TEST_USERMESSAGEFIELD, m_cfield);
	DDX_Control(pDX, IDC_TEST_USERMESSAGE, m_cusermessage);
	//}}AFX_DATA_MAP
	DDX_Control(pDX, IDC_ENCODERDISTANCE, m_cdistance);
	DDX_Control(pDX, IDC_ENCODERSPEED2, m_cspeed2);
	DDX_Control(pDX, IDC_ENCODERDISTANCE2, m_cdistance2);
	DDX_Control(pDX, IDC_EDIT_ACTSIZE, m_cactsize);
	DDX_Control(pDX, IDC_EDIT_FILLSTATUS, m_cfillstatus);
	DDX_Control(pDX, IDC_EDIT1, m_ccnts);
	DDX_Control(pDX, IDC_EDIT2, m_cmaxtics);
	DDX_Control(pDX, IDC_EDIT3, m_cmesspersec);
	DDX_Control(pDX, IDC_EDIT_FILLSTATUS2, m_cminfillstatus);
	DDX_Control(pDX, IDC_TEST_BUFFERSIZE2, m_climit);
	DDX_Control(pDX, IDC_EDIT_MS1, m_cwait1);
	DDX_Control(pDX, IDC_EDIT_MS2, m_cwait2);
	DDX_Control(pDX, IDC_EDIT_MS3, m_cwait3);
	DDX_Control(pDX, IDC_EDIT_MS4, m_cppersec);
}

BEGIN_MESSAGE_MAP(CSocketCommCPPTestDlg, CDialog)
	//{{AFX_MSG_MAP(CSocketCommCPPTestDlg)
	ON_WM_SYSCOMMAND()
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_BN_CLICKED(IDC_TEST_LASER, OnTestLaser)
	ON_BN_CLICKED(IDC_TEST_CONNECT, OnTestConnect)
	ON_BN_CLICKED(IDC_TEST_DISCONNECT, OnTestDisconnect)
	ON_WM_DESTROY()
	ON_BN_CLICKED(IDC_TEST_LASER2, OnTestLaser2)
	ON_BN_CLICKED(IDC_TEST_BUFFEREDUM, OnTestBufferedum)
	ON_BN_CLICKED(IDC_TEST_POWERSCALE0, OnTestPowerscale0)
	ON_BN_CLICKED(IDC_TEST_POWERSCALE1, OnTestPowerscale1)
	ON_BN_CLICKED(IDC_TEST_POWERSCALE2, OnTestPowerscale2)
	ON_BN_CLICKED(IDC_TEST_POWERSCALE3, OnTestPowerscale3)
	ON_BN_CLICKED(IDC_TEST_DYNAMICVELOCITY, OnTestDynamicvelocity)
	ON_BN_CLICKED(IDC_TEST_LASER_GETUM, OnTestLaserGetum)
	ON_BN_CLICKED(IDC_TESTSTOP, OnTestStop)
	//}}AFX_MSG_MAP
	ON_BN_CLICKED(IDC_TEST_DYNAMICDISTANCE, &CSocketCommCPPTestDlg::OnBnClickedTestDynamicdistance)
	ON_BN_CLICKED(IDC_BUTTON2, &CSocketCommCPPTestDlg::OnBnClickedButton2)
	ON_WM_TIMER()
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CSocketCommCPPTestDlg message handlers

BOOL CSocketCommCPPTestDlg::OnInitDialog()
{
	CDialog::OnInitDialog();

	// Add "About..." menu item to system menu.

	// IDM_ABOUTBOX must be in the system command range.
	ASSERT((IDM_ABOUTBOX & 0xFFF0) == IDM_ABOUTBOX);
	ASSERT(IDM_ABOUTBOX < 0xF000);

	CMenu* pSysMenu = GetSystemMenu(FALSE);
	if (pSysMenu != NULL)
	{
		CString strAboutMenu;
		strAboutMenu.LoadString(IDS_ABOUTBOX);
		if (!strAboutMenu.IsEmpty())
		{
			pSysMenu->AppendMenu(MF_SEPARATOR);
			pSysMenu->AppendMenu(MF_STRING, IDM_ABOUTBOX, strAboutMenu);
		}
	}

	// Set the icon for this dialog.  The framework does this automatically
	//  when the application's main window is not a dialog
	SetIcon(m_hIcon, TRUE);			// Set big icon
	SetIcon(m_hIcon, FALSE);		// Set small icon
	
	// TODO: Add extra initialization here
	CString s;
	m_cipaddress.SetWindowText(L"192.168.000.180");
	m_cbuffersize.SetWindowText(L"1000");
	m_cdisconnect.EnableWindow(FALSE);
	m_ctest.EnableWindow(FALSE);
	m_ctest2.EnableWindow(FALSE);
	m_cteststop.EnableWindow(FALSE);	
	m_czerooffset.SetCheck(FALSE);

	m_cscale0.EnableWindow(FALSE);
	m_cscale1.EnableWindow(FALSE);
	m_cscale2.EnableWindow(FALSE);
	m_cscale3.EnableWindow(FALSE);
	m_cpercentage.SetWindowText(L"100");
	
	s.Format(_T("%d"),m_filllimit);
	m_climit.SetWindowText(s);

	m_cfield.SetWindowText(_T("0"));
	m_cusermessage.SetWindowText(_T("abcdefghijk"));
	
	m_cppersec.SetWindowText(_T("100"));

	//init global threaddata
	g_threaddata.sock = 0;
	g_threaddata.bdatastring = 0;
	g_threaddata.bend = 0;
	g_threaddata.field = 0;
	g_threaddata.framecnt = 0;
	g_threaddata.limit = 0;
	g_threaddata.maxtics = 0;
	g_threaddata.wait1 = 50;
	g_threaddata.wait2 = 10;
	g_threaddata.wait3 = 2;

	SetTimer(TIMER_ID,1000,NULL);
	return TRUE;  // return TRUE  unless you set the focus to a control
}

void CSocketCommCPPTestDlg::OnSysCommand(UINT nID, LPARAM lParam)
{
	if ((nID & 0xFFF0) == IDM_ABOUTBOX)
	{
		CAboutDlg dlgAbout;
		dlgAbout.DoModal();
	}
	else
	{
		CDialog::OnSysCommand(nID, lParam);
	}
}

// If you add a minimize button to your dialog, you will need the code below
//  to draw the icon.  For MFC applications using the document/view model,
//  this is automatically done for you by the framework.

void CSocketCommCPPTestDlg::OnPaint() 
{
	if (IsIconic())
	{
		CPaintDC dc(this); // device context for painting

		SendMessage(WM_ICONERASEBKGND, (WPARAM) dc.GetSafeHdc(), 0);

		// Center icon in client rectangle
		int cxIcon = GetSystemMetrics(SM_CXICON);
		int cyIcon = GetSystemMetrics(SM_CYICON);
		CRect rect;
		GetClientRect(&rect);
		int x = (rect.Width() - cxIcon + 1) / 2;
		int y = (rect.Height() - cyIcon + 1) / 2;

		// Draw the icon
		dc.DrawIcon(x, y, m_hIcon);
	}
	else
	{
		CDialog::OnPaint();
	}
}

// The system calls this to obtain the cursor to display while the user drags
//  the minimized window.
HCURSOR CSocketCommCPPTestDlg::OnQueryDragIcon()
{
	return (HCURSOR) m_hIcon;
}

void CSocketCommCPPTestDlg::OnTestConnect() 
{
	// TODO: Add your control notification handler code here
	wchar_t laser_ip[16];
	int i,j;
	CString s;
	m_cipaddress.GetWindowText(s);
	wcsncpy(laser_ip,s,15);
	sock = 0;
	//just in case
	g_threaddata.bend = 1;
	Sleep(50);
	g_threaddata.sock = 0;

	// initialize and connect
	MInit(sock,L"anyword", laser_ip, L".\\");
	i = MStartClient(sock);

	MInit(g_threaddata.sock,L"anyword", laser_ip, L".\\");
	j = MStartClient(g_threaddata.sock);


	if(i==0 && j==0){
		m_cconnect.EnableWindow(FALSE);
		m_cdisconnect.EnableWindow(TRUE);
		m_ctest.EnableWindow(TRUE);
		m_ctest2.EnableWindow(TRUE);

		m_cscale0.EnableWindow(TRUE);
		m_cscale1.EnableWindow(TRUE);
		m_cscale2.EnableWindow(TRUE);
		m_cscale3.EnableWindow(TRUE);
	}else{
		MFinish(sock);
		MFinish(g_threaddata.sock);
		sock = 0;
		g_threaddata.sock = 0;
	}

}

void CSocketCommCPPTestDlg::OnTestLaser() 
{
	// TODO: Add your control notification handler code here
	int i;

	//usermessage
	char usermess[256];
	char fieldtext[4];
	int field = 0;
	int len = 0;
	int usedatastring  = 0;
	char datastring[1024];
	CString s;

	memset(usermess,0,256);
	memset(fieldtext,0,4);
	
	m_cusermessage.GetWindowText(s);
	for(i = 0; i < s.GetLength();i++){
		usermess[i] = (char) s.GetAt(i);
	}
	m_cfield.GetWindowText(s);
	for(i = 0; i < s.GetLength();i++){
		fieldtext[i] = (char) s.GetAt(i);
	}
	
	usedatastring = m_cdatastring.GetCheck();

	if(strlen(fieldtext)){
		field = atoi(fieldtext);
		m_field = field;
		if(usedatastring){
			char *pd = new char[20000];
			len = 20000;
			if (pd) {
				memset(pd, 0, len);
				i = MLaser_FastDataString(sock, field, pd, len);
				delete[] pd;
			}
/*
			len = strlen(usermess);
			if(len > 255) len = 255;
			memset(datastring,0,len);
			memcpy(datastring+len,usermess,len);
			memset(datastring+2*len,0xff,len);
			len *= 3;
			i = MLaser_FastDataString(sock,field,datastring,len);
			*/
		}else{
			i = MLaser_FastUsermessage(sock,field,usermess);
		}

	}

}


void CSocketCommCPPTestDlg::OnTestDisconnect() 
{
	// TODO: Add your control notification handler code here
	// shutdown
	int	i,j;
	int value = 1000;
	//undo eventual power scaling
	for(j = 0; j < 4; j++){
		if(!MIsConnected(sock)) break;
		i = MLaser_Powerscale(sock,0,j,value);
		
	}
	if(MIsConnected(sock)){
		i = MLaser_Knockout(sock);
		i = MShutdownClient(sock);
	}
	MFinish(sock);
	sock = 0;
	//end thread in a horrible way
	g_threaddata.bend = 1;
	Sleep(100);
	MFinish(g_threaddata.sock);
	g_threaddata.sock = 0;

	m_cconnect.EnableWindow(TRUE);
	m_cdisconnect.EnableWindow(FALSE);
	m_ctest.EnableWindow(FALSE);
	m_ctest2.EnableWindow(FALSE);

	m_cscale0.EnableWindow(FALSE);
	m_cscale1.EnableWindow(FALSE);
	m_cscale2.EnableWindow(FALSE);
	m_cscale3.EnableWindow(FALSE);


}

void CSocketCommCPPTestDlg::OnDestroy() 
{
	CDialog::OnDestroy();
	
	// TODO: Add your message handler code here
	int i;
	i = MLaser_Knockout(sock);
	i = MShutdownClient(sock);
	MFinish(sock);
	//end it in a horrible way
	g_threaddata.bend = 1;
	Sleep(100);
	MFinish(g_threaddata.sock);
}

void CSocketCommCPPTestDlg::OnTestLaser2() 
{
	// TODO: Add your control notification handler code here
	int i;
	//Shift all messages by absolute offset
	int dx,dy;//offset value
	int relative = 0;//absolute value
	int format = 1;//units (um)
	int reset = m_czerooffset.GetCheck() != 0 ? 1: 0;//no reset after print
	CString s;
	char value[25];
	dx = 0; dy = 0;
	
	memset(value,0,25);
	m_coffsetx.GetWindowText(s);
	for(i = 0; i < s.GetLength();i++){
		value[i] = (char) s.GetAt(i);
	}
	dx = atoi(value);
	memset(value,0,25);
	m_coffsety.GetWindowText(s);
	for(i = 0; i < s.GetLength();i++){
		value[i] = (char) s.GetAt(i);
	}
	dy = atoi(value);
	
	i = MLaser_Offset(sock,dx,dy,relative,format,reset);

	
}


void CSocketCommCPPTestDlg::OnTestBufferedum() 
{
	// TODO: Add your control notification handler code here
	int i,prevent,full,fillstatus;
	int actsize = 0;
	//usermessage
	char usermess[256];
	char fieldtext[4];
	char datastring[1024];
	CString s;
	int field = 0;
	int size;
	CString txt;
	int usedatastring = FALSE;
	int limit;
	int wait1,wait2,wait3; //sleep in ms
	//buffer size
	m_cbuffersize.GetWindowText(s);
	size = _ttoi(s);
	//limit
	m_climit.GetWindowText(s);
	m_filllimit = _ttoi(s);
	limit = (size * m_filllimit) / 100;
	//calculate wait cycles
	int ppersec;float fms;
	m_cppersec.GetWindowText(s);
	ppersec = _ttoi(s);
	if(ppersec < 1) ppersec = 1;
	//time per print
	fms = 1000.0f/ppersec;
	//queue < limit
	wait3 = (fms/2.0f);
	//queue > limit
	wait2 = fms;
	//full queue: wait until depleted to between limit and full size
	if(size > limit){
		wait1 = ((1000.0f*((float)size-(float)limit))/ppersec)/2.0f;
	}else{
		wait1 = ((1000.0f*((float)size))/ppersec)/2.0f;
	}
	s.Format(_T("%d"),wait1);
	m_cwait1.SetWindowText(s);
	s.Format(_T("%d"),wait2);
	m_cwait2.SetWindowText(s);
	s.Format(_T("%d"),wait3);
	m_cwait3.SetWindowText(s);

	memset(usermess,0,256);
	memset(fieldtext,0,4);
	memset(datastring,0,1024);


	m_cusermessage.GetWindowText(s);
	for(i = 0; i < s.GetLength();i++){
		usermess[i] = (char) s.GetAt(i);
	}

	m_cfield.GetWindowText(s);
	for(i = 0; i < s.GetLength();i++){
		fieldtext[i] = (char) s.GetAt(i);
	}
	

	txt = usermess;
	//enable buffered usermessage (10 buffers enabled)
	m_ctestbufferedum.EnableWindow(FALSE);
	m_cteststop.EnableWindow(TRUE);


	prevent = 0;
	full = 0;
	m_minfillstatus = -1;

	usedatastring = m_cdatastring.GetCheck();

	if(usedatastring){
		int field;
		field = fillstatus = 0;
		i = MLaser_EnableBufferedDataString(sock,0,actsize,field,fillstatus,size);
	}else{
		i = MLaser_EnableBufferedUM(sock,0,actsize,size);
	}


	
	//init global threaddata
	g_threaddata.bdatastring = usedatastring;
	g_threaddata.bend = 0;
	g_threaddata.field = field;
	g_threaddata.framecnt = 0;
	g_threaddata.limit = limit;
	g_threaddata.maxtics = 0;
	g_threaddata.wait1 = wait1;
	g_threaddata.wait2 = wait2;
	g_threaddata.wait3 = wait3;

	startprintthread();
	/*

	if(strlen(fieldtext)){
		int j; int k = 0;DWORD t1,t2;
		MSG mis;
		field = atoi(fieldtext);
		m_field = field;
		int out = 0;
		int len = 0;
		m_cnts = m_lastcnts = 0;
		m_maxtics = 0;
		t1 = GetTickCount();
		do{
			if(!prevent){
				if(usedatastring){
					if(k>9) k = 0;
					len = (k + 1) * 10;
					memset(datastring,0xFF,1024);//all white
					memset(datastring,0,len);//len bytes black
					len = 1024;
					i = MLaser_FastDataString(sock,field,datastring,len);
				}else{
					if(k%2){
						sprintf(usermess,"%s%010u",txt,k);
					}else{
						sprintf(usermess,"ABC%s%010u",txt,k);
					}
					i = MLaser_FastUsermessage(sock,field,usermess);
				}
			}else{
				i = 0;
			}
			if(i==8){
				full = TRUE;
				if(m_minfillstatus == -1) m_minfillstatus = actsize;//init minimum
			}else if(i){
				out = 1;
				if(usedatastring){
					txt.Format(L"Error:FastDataString returns <%ld>",i);
				}else{
					txt.Format(L"Error:FastUserMessage returns <%ld>",i);
				}
				AfxMessageBox(txt);
			}
			else{
				k++;
				m_cnts++;
				t2 = GetTickCount();
				if(t2 > t1){
					if(t2 - t1 > m_maxtics){
						m_maxtics = t2 - t1;
					}
				}
				t1 = t2;
			}
			if(full){
				prevent = 1;
			}
			if(k > 9999999999) k = 0;
			fillstatus = 0;
			if(usedatastring){
				//get fill status
				i = MLaser_EnableBufferedDataString(sock,1,actsize,field,fillstatus,0);
			}else{
				//get actual fillstatus
				i = MLaser_EnableBufferedUMExt(sock, 1,actsize,field, fillstatus,0);
			}
			if(m_minfillstatus != -1){
				if(fillstatus < m_minfillstatus) m_minfillstatus = fillstatus;
			}
			//enable sending
			if(fillstatus < limit){
				prevent = 0;
				full = 0;
			}
			if(full) Sleep(wait1);
			else if(prevent) Sleep(wait2);
			else Sleep(wait3);
			while(PeekMessage(&mis, NULL, 0, 65535, PM_REMOVE)){
				TranslateMessage(&mis);
				DispatchMessage(&mis);
				if(mis.hwnd==m_cteststop.m_hWnd){
					if (mis . message == WM_KEYDOWN ||
						mis . message == WM_LBUTTONDOWN){
						out = 1;
					}
				}
			}
		}while(!out);
	}

	//disable buffered Usermessage
	if(usedatastring){
		int field, fillstatus;
		field = fillstatus = 0;
		i = MLaser_EnableBufferedDataString(sock,0,actsize,field,fillstatus,0);
	}else{
		i = MLaser_EnableBufferedUM(sock,0,actsize,0);
	}
	m_ctestbufferedum.EnableWindow(TRUE);
	m_cteststop.EnableWindow(FALSE);


	*/
}

void CSocketCommCPPTestDlg::OnTestStop() {
	int i;int actsize = 0;int fillstatus = 0;
	g_threaddata.bend = 1;
	Sleep(100);
	//disable
	i = MLaser_EnableBufferedDataString(sock,0,actsize,g_threaddata.field,fillstatus,0);
	i = MLaser_EnableBufferedDataString(sock,0,actsize,g_threaddata.field,fillstatus,0);
	m_ctestbufferedum.EnableWindow(TRUE);
	m_cteststop.EnableWindow(FALSE);

}


void CSocketCommCPPTestDlg::OnTestPowerscale0() 
{
	// TODO: Add your control notification handler code here
	int i;
	CString s;
	int value;
	int member = 0;//bitmap time of pixel
	
	m_cpercentage.GetWindowText(s);
	value = _ttol(s)* 10;//send in permille
	i = MLaser_Powerscale(sock,0,member,value);

}

void CSocketCommCPPTestDlg::OnTestPowerscale1() 
{
	// TODO: Add your control notification handler code here
	CString s;
	int value;
	int member = 1;//bitmap power of pixel (makes sense for YAG only)
	int i;
	m_cpercentage.GetWindowText(s);
	value = _ttol(s)* 10;//send in permille
	i = MLaser_Powerscale(sock,0,member,value);
	
}

void CSocketCommCPPTestDlg::OnTestPowerscale2() 
{
	// TODO: Add your control notification handler code here
	CString s;
	int value;
	int member = 2;//layer vectorial speed
	int i;
	m_cpercentage.GetWindowText(s);
	value = _ttol(s)* 10;//send in permille
	i = MLaser_Powerscale(sock,0,member,value);
	
}

void CSocketCommCPPTestDlg::OnTestPowerscale3() 
{
	// TODO: Add your control notification handler code here
	CString s;
	int value;
	int member = 3;//layer power
int i;	
	m_cpercentage.GetWindowText(s);
	value = _ttol(s)* 10;//send in permille
	i = MLaser_Powerscale(sock,0,member,value);
	
}

void CSocketCommCPPTestDlg::OnTestDynamicvelocity() 
{
	// TODO: Add your control notification handler code here
	CString s;
	m_cencoderspeed.GetWindowText(s);
	int i;	
	int speed = (int)(1000.0f*_wtof(s));
	int var = 1;//we set the speed
	if(speed <= 1000) speed = 1000;//set some lower limit
	i = MLaser_SetDynamic(sock,var,speed);

}

void CSocketCommCPPTestDlg::OnTestLaserGetum() 
{
	// TODO: Add your control notification handler code here
	//usermessage
	int len = 1024;
	int i;
	char buf[2048];
	char fieldtext[4];
	int field = 0;
	CString s;
	int usedatastring = 0;
	memset(buf,0,len);
	memset(fieldtext,0,4);
	
	m_cfield.GetWindowText(s);
	for(i = 0; i < s.GetLength();i++){
		fieldtext[i] = (char)s.GetAt(i);
	}
	usedatastring = m_cdatastring.GetCheck();

	if(strlen(fieldtext)){
		field = atoi(fieldtext);
		if(usedatastring){
			len = 2048;
			i = MLaser_GetFastDataString(sock,field,buf,len);
			if(i==0) {
				CString txt;CString t;
				for(i = 0; i < len; i++){
					t.Format(L"%02X",buf[i]);
					txt += t;
				}
				t.Format(L"Field: %ld, Stringlength=%ld Binarydata <%s>",field,len,txt);
				AfxMessageBox(t);
			}
		}else{
			i = MLaser_GetFastUsermessage(sock,field,buf,len);
			if(i==0) {
				CString txt;
				txt.Format(L"Field: %ld, String <%s>, Stringlength=%ld",field,CString(buf),len-1);
				AfxMessageBox(txt);
			}
		}
	}
	{
		int outlen = 100; char out[101];
		int elements = 0;
		int index = 0;
		int field = 0;
		i = MLaser_GetFifofield(sock, field, index,out,outlen, elements);
	}
}

void CSocketCommCPPTestDlg::OnBnClickedTestDynamicdistance()
{
	// TODO: Add your control notification handler code here
	CString s;
	m_cdistance.GetWindowText(s);
	int i;	
	int distance = _wtof(s)*1000.0;
	int var = 0;//we set the distance
	if(distance < 0) distance = 0;//set some lower limit
	i = MLaser_SetDynamic(sock,var,distance);

}

void CSocketCommCPPTestDlg::OnBnClickedButton2()
{
	// TODO: Add your control notification handler code here
	CString s;
	int i;	
	int distance;
	
	i = MLaser_GetDynamic(sock,0,distance);
	s.Format(_T("%.3f"),(float)distance/1000.0f);
	if(i==0)m_cdistance2.SetWindowText(s);

	i = MLaser_GetDynamic(sock,1,distance);
	s.Format(_T("%.2f"),(float)distance/1000.0f);
	if(i==0) m_cspeed2.SetWindowText(s);

}

void CSocketCommCPPTestDlg::OnTimer(UINT_PTR nIDEvent)
{
	// TODO: Add your message handler code here and/or call default
	CString s; DWORD t,delta;
	int actsize = 0;
	int field;int fillstatus = 0;
	unsigned int cnts;
	int maxtics;

	//data from the thread
	cnts = g_threaddata.framecnt;
	maxtics = g_threaddata.maxtics;
	field = g_threaddata.field;

	delta = 0;
	if(MIsConnected(sock)){
		//get the fillstatus
		if(MLaser_EnableBufferedUMExt(sock, 1,actsize,field, fillstatus,0)==0){
			if(m_minfillstatus != -1){
				if(fillstatus < m_minfillstatus) m_minfillstatus = fillstatus;
			}
		};
	}
	s.Format(_T("%d"),actsize);
	m_cactsize.SetWindowText(s);
	s.Format(_T("%d"),fillstatus);
	m_cfillstatus.SetWindowText(s);
	

	s.Format(_T("%u"),cnts);
	m_ccnts.SetWindowText(s);
	//data from the thread
	s.Format(_T("%d"),maxtics);
	m_cmaxtics.SetWindowText(s);

	s.Format(_T("%d"),m_minfillstatus);
	m_cminfillstatus.SetWindowText(s);
	

	t = GetTickCount();
	if(m_ticstimer==0){
		m_ticstimer = t;
	}else if(t > m_ticstimer){
		delta = t - m_ticstimer;
		m_ticstimer = t;
	}
	if(delta){
		float npersec;
		if(cnts > m_lastcnts){
			npersec = (float)(cnts - m_lastcnts)/(float)delta*1000.0f;
			s.Format(_T("%.1f"),npersec);
			m_cmesspersec.SetWindowText(s);
		}
	}
	m_lastcnts = cnts;

	CDialog::OnTimer(nIDEvent);
}

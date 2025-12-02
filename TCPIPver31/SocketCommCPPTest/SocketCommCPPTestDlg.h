// SocketCommCPPTestDlg.h : header file
//

#include "afxwin.h"
#if !defined(AFX_SOCKETCOMMCPPTESTDLG_H__BE5CB646_44B4_11DA_A567_00010316EA7C__INCLUDED_)
#define AFX_SOCKETCOMMCPPTESTDLG_H__BE5CB646_44B4_11DA_A567_00010316EA7C__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

/////////////////////////////////////////////////////////////////////////////
// CSocketCommCPPTestDlg dialog

class CSocketCommCPPTestDlg : public CDialog
{
// Construction
public:
	CSocketCommCPPTestDlg(CWnd* pParent = NULL);	// standard constructor

// Dialog Data
	//{{AFX_DATA(CSocketCommCPPTestDlg)
	enum { IDD = IDD_SOCKETCOMMCPPTEST_DIALOG };
	CButton	m_cdatastring;
	CEdit	m_cencoderspeed;
	CEdit	m_cpercentage;
	CButton	m_cscale3;
	CButton	m_cscale2;
	CButton	m_cscale1;
	CButton	m_cscale0;
	CButton	m_czerooffset;
	CEdit	m_cbuffersize;
	CButton	m_cteststop;
	CButton	m_ctestbufferedum;
	CIPAddressCtrl	m_cipaddress;
	CButton	m_ctest2;
	CEdit	m_coffsety;
	CEdit	m_coffsetx;
	CButton	m_ctest;
	CButton	m_cdisconnect;
	CButton	m_cconnect;
	CEdit	m_cfield;
	CEdit	m_cusermessage;
	//}}AFX_DATA

	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CSocketCommCPPTestDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	HICON m_hIcon;

	// Generated message map functions
	//{{AFX_MSG(CSocketCommCPPTestDlg)
	virtual BOOL OnInitDialog();
	afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	afx_msg void OnTestLaser();
	afx_msg void OnTestConnect();
	afx_msg void OnTestDisconnect();
	afx_msg void OnDestroy();
	afx_msg void OnTestLaser2();
	afx_msg void OnTestBufferedum();
	afx_msg void OnTestPowerscale0();
	afx_msg void OnTestPowerscale1();
	afx_msg void OnTestPowerscale2();
	afx_msg void OnTestPowerscale3();
	afx_msg void OnTestDynamicvelocity();
	afx_msg void OnTestLaserGetum();
	afx_msg void OnTestStop();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
public:
	afx_msg void OnBnClickedTestDynamicdistance();
public:
	CEdit m_cdistance;
public:
	afx_msg void OnBnClickedButton2();
public:
	CEdit m_cspeed2;
public:
	CEdit m_cdistance2;
public:
	afx_msg void OnTimer(UINT_PTR nIDEvent);
public:
	CEdit m_cactsize;
public:
	CEdit m_cfillstatus;
public:
	CEdit m_ccnts;
public:
	CEdit m_cmaxtics;
public:
	CEdit m_cmesspersec;
public:
	CEdit m_cminfillstatus;
public:
	CEdit m_climit;
public:
	CEdit m_cwait1;
public:
	CEdit m_cwait2;
public:
	CEdit m_cwait3;
public:
	CEdit m_cppersec;
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_SOCKETCOMMCPPTESTDLG_H__BE5CB646_44B4_11DA_A567_00010316EA7C__INCLUDED_)

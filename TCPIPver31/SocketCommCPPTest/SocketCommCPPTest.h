// SocketCommCPPTest.h : main header file for the SOCKETCOMMCPPTEST application
//

#if !defined(AFX_SOCKETCOMMCPPTEST_H__BE5CB644_44B4_11DA_A567_00010316EA7C__INCLUDED_)
#define AFX_SOCKETCOMMCPPTEST_H__BE5CB644_44B4_11DA_A567_00010316EA7C__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#ifndef __AFXWIN_H__
	#error include 'stdafx.h' before including this file for PCH
#endif

#include "resource.h"		// main symbols

/////////////////////////////////////////////////////////////////////////////
// CSocketCommCPPTestApp:
// See SocketCommCPPTest.cpp for the implementation of this class
//

class CSocketCommCPPTestApp : public CWinApp
{
public:
	CSocketCommCPPTestApp();

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CSocketCommCPPTestApp)
	public:
	virtual BOOL InitInstance();
	//}}AFX_VIRTUAL

// Implementation

	//{{AFX_MSG(CSocketCommCPPTestApp)
		// NOTE - the ClassWizard will add and remove member functions here.
		//    DO NOT EDIT what you see in these blocks of generated code !
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};


/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_SOCKETCOMMCPPTEST_H__BE5CB644_44B4_11DA_A567_00010316EA7C__INCLUDED_)

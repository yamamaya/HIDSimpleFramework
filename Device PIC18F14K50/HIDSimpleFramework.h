#ifndef __HIDSimpleFramework_h__
#define __HIDSimpleFramework_h__

#include "GenericTypeDefs.h"
#include "HardwareProfile.h"
#include "AppLib/USB/usb.h"
#include "AppLib/USB/usb_function_hid.h"

void init( void );
void run( void );

// Send data to host.
// The data data should be less than or equal to 64 bytes.
void HIDSend( BYTE *buff, int len );

// Receive data from host.
// buff and len should be enough size to receive, otherwise the data will be truncated.
// The maximum size of data is 64 bytes.
BOOL HIDReceive( BYTE *buff, int len );

#endif

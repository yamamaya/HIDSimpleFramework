#ifndef __HIDSimpleFramework_h__
#define __HIDSimpleFramework_h__

#include "Compiler.h"
#include "GenericTypeDefs.h"
#include "HardwareProfile.h"

void init( void );
void run( void );

// Send data to host.
// The data data should be less than or equal to 64 bytes.
void HIDSend( BYTE *buff, int len );

// Retrieve send buffer status.
// If send buffer is empty (the data in the buffer is already sent), returns true, otherwise returns false.
BOOL HIDSendReady( void );

// Receive data from host.
// buff and len should be enough size to receive, otherwise the data will be truncated.
// The maximum size of data is 64 bytes.
BOOL HIDReceive( BYTE *buff, int len );

#endif

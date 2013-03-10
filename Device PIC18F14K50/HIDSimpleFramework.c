#include <string.h>
#include "config.h"
#include "AppLib/USB/usb.h"
#include "AppLib/USB/usb_function_hid.h"
#include "HIDSimpleFramework.h"

USB_HANDLE USBOutHandle;
USB_HANDLE USBInHandle;

void USBCB_SOF_Handler();
void USBCBSuspend();
void USBCBWakeFromSuspend();
void USBCBInitEP();
void USBCBStdSetDscHandler();
void USBCBCheckOtherReq();
void USBCBErrorHandler();

void High_ISR_Main();
void Low_ISR_Main();

#pragma code HIGH_INTERRUPT_VECTOR = 0x08
void High_ISR( void ) {
	_asm
		goto High_ISR_Main
	_endasm
}

#pragma code LOW_INTERRUPT_VECTOR = 0x18
void Low_ISR( void ) {
	_asm
		goto Low_ISR_Main
	_endasm
}

#pragma code

#pragma interrupt High_ISR_Main
void High_ISR_Main() {
	if ( PIR2bits.USBIF ) {
		USBDeviceTasks();
		PIR2bits.USBIF = 0;
	}
	// Insert your ISR code here.
}

#pragma interruptlow Low_ISR_Main
void Low_ISR_Main() {
	// Insert your ISR code here.
}

#pragma code

void HIDSend( BYTE *buff, int len ) {
	while ( HIDTxHandleBusy( USBInHandle ) ) {
	}
	memset( (void *)hid_report_in, 0, HID_INT_IN_EP_SIZE );
	if ( len > HID_INT_IN_EP_SIZE ) {
		len = HID_INT_IN_EP_SIZE;
	}
	memcpy( (void *)hid_report_in, (void *)buff, len );
	USBInHandle = HIDTxPacket( HID_EP, (BYTE *)hid_report_in ,HID_INT_IN_EP_SIZE );
}

BOOL HIDSendReady( void ) {
	return ! HIDTxHandleBusy( USBInHandle );
}

BOOL HIDReceive( BYTE *buff, int len ) {
	if( USBDeviceState >= CONFIGURED_STATE && USBSuspendControl != 1 ) {
		if ( ! HIDRxHandleBusy( USBOutHandle ) ) {
			if ( len > HID_INT_OUT_EP_SIZE ) {
				len = HID_INT_OUT_EP_SIZE;
			}
			memcpy( (void *)buff, (void *)hid_report_out, len );
			USBOutHandle = HIDRxPacket( HID_EP, (BYTE *)hid_report_out, HID_INT_OUT_EP_SIZE );
			return TRUE;
		}
	}
	return FALSE;
}

void main() {
	WORD w1, w2;

	USBOutHandle = 0;
	USBInHandle = 0;

	RCONbits.IPEN = 1;			// Interrupt Priority: Enable priority levels on interrupts
	INTCONbits.GIEH = 1;		// Enable high-priority interrupts
	INTCONbits.GIEL = 1;		// Enable low-priority interrupts

	init();

	USBDeviceInit();
	for ( w1 = 0 ; w1 < 50000 ; w1 ++ ) {
		for ( w2 = 0 ; w2 < 10 ; w2 ++ ) {
			Nop();
		}
	}
	USBDeviceAttach();

	while ( 1 ) {
		run();
	}
}

void USBCB_SOF_Handler() {
// The USB host sends out a SOF packet to full-speed devices every 1 ms.
// This interrupt may be useful for isochronous pipes. End designers should implement callback routine as necessary.
}

void USBCBSuspend() {
// This call back is invoked when a USB suspend is detected.
}

void USBCBWakeFromSuspend() {
// The host may put USB peripheral devices in low power suspend mode (by "sending" 3+ms of idle).
// Once in suspend mode, the host may wake the device back up by sending non-idle state signalling.
// This call back is invoked when a wakeup from USB suspend is detected.
}

void USBCBInitEP() {
    USBEnableEndpoint( HID_EP,USB_IN_ENABLED|USB_OUT_ENABLED|USB_HANDSHAKE_ENABLED|USB_DISALLOW_SETUP );
    USBOutHandle = HIDRxPacket( HID_EP, (BYTE *)hid_report_out, HID_INT_OUT_EP_SIZE );
}

void USBCBStdSetDscHandler() {
}

void USBCBCheckOtherReq() {
    USBCheckHIDRequest();
}

void USBCBErrorHandler() {
// The purpose of this callback is mainly for debugging during development.
// Check UEIR to see which error causes the interrupt.
}

BOOL USER_USB_CALLBACK_EVENT_HANDLER( USB_EVENT event, void *pdata, WORD size ) {
    switch( event ) {
        case EVENT_TRANSFER:
            //Add application specific callback task or callback function here if desired.
            break;
        case EVENT_SOF:
            USBCB_SOF_Handler();
            break;
        case EVENT_SUSPEND:
            USBCBSuspend();
            break;
        case EVENT_RESUME:
            USBCBWakeFromSuspend();
            break;
        case EVENT_CONFIGURED: 
            USBCBInitEP();
            break;
        case EVENT_SET_DESCRIPTOR:
            USBCBStdSetDscHandler();
            break;
        case EVENT_EP0_REQUEST:
            USBCBCheckOtherReq();
            break;
        case EVENT_BUS_ERROR:
            USBCBErrorHandler();
            break;
        case EVENT_TRANSFER_TERMINATED:
            break;
        default:
            break;
    }      
    return TRUE; 
}

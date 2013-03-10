#include "HIDSimpleFramework.h"

// RB0 --> push switch --> GND
// RB1 --> R --> LED --> GND

void init( void ) {
	ANSELBbits.ANSB0 = 0;	// configure RB0 as digital input
	TRISBbits.TRISB0 = 1;
	WPUB = 0x01;			// Enable weak pull-up for only RB0
	INTCON2bits.RBPU = 0;	// enable PORTB pull-ups

	TRISBbits.TRISB1 = 0;	// configure RB0 as digital output
}

void run( void ) {
	BYTE buff[64];

	if ( HIDReceive( buff, sizeof( buff ) ) ) {
		switch ( buff[0] ) {
			case 0x00: // Set LED
				if ( buff[1] ) {
					LATBbits.LATB1 = 1;
				} else {
					LATBbits.LATB1 = 0;
				}
				buff[0] = LATBbits.LATB1;
				HIDSend( buff, 1 );
				break;
	
			case 0x01: // Read switch status
				if ( PORTBbits.RB0 ) {
					buff[0] = 0;
				} else {
					buff[0] = 1;
				}
				HIDSend( buff, 1 );
				break;
	
			default:
				break;
		}
	}
}

void HighISR( void ) {
	// High priority interrupt service routine
}

void LowISR( void ) {
	// Low priority interrupt service routine
}

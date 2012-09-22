#include "HIDSimpleFramework.h"

// RB4 --> push switch --> GND
// RB5 --> R --> LED --> GND
// RC0(AN4) --> potentiometer

WORD ReadAnalogPort( BYTE ch ) {
	ADCON0 = ( ( ch ) << 2 ) | 0x03;
	while ( ADCON0bits.GO ) {
	}
	return ( (WORD)ADRESH << 8 ) | ADRESL;
}

void init( void ) {
	ANSEL = 0b00010000;	// configure AN4 as an analog input, others as digital I/Os
	ANSELH = 0b00000000;
	INTCON2bits.RABPU = 0;	// enable PORTA and PORTB pull-ups
	TRISA = 0b11111111;	// configure all PORTA as inputs
	WPUAbits.WPUA3 = 1;	// enable RA3 pull-up
	LATB = 0;
	TRISB = 0b00010000;	// configure RB4 as input, others as outputs
	WPUBbits.WPUB4 = 1;	// enable RB4 pull-up
	TRISC = 0b10000001;	// configure RC0(AN4) as input, others as outputs

	ADCON1 = 0;				// Positive Voltage Reference: VDD, Negative Voltage Reference: VSS
	ADCON2bits.ADCS0 = 0;		// A/D Conversion Clock: Fosc/64
	ADCON2bits.ADCS1 = 1;
	ADCON2bits.ADCS2 = 1;
	ADCON2bits.ACQT0 = 1;		// Acquisition time: 20TAD
	ADCON2bits.ACQT1 = 1;
	ADCON2bits.ACQT2 = 1;
	ADCON2bits.ADFM = 1;		// A/D Conversion Result Format: Right justified
	ADCON0bits.ADON = 1;		// Enable ADC
}

void run( void ) {
	BYTE buff[64];
	WORD w;

	if ( HIDReceive( buff, sizeof( buff ) ) ) {
		switch ( buff[0] ) {
			case 0x00: // Set LED
				if ( buff[1] ) {
					LATBbits.LATB5 = 1;
				} else {
					LATBbits.LATB5 = 0;
				}
				buff[0] = LATBbits.LATB5;
				HIDSend( buff, 1 );
				break;
	
			case 0x01: // Read switch status
				if ( PORTBbits.RB4 ) {
					buff[0] = 0;
				} else {
					buff[0] = 1;
				}
				HIDSend( buff, 1 );
				break;
	
			case 0x02: // Read ADC
				w = ReadAnalogPort( 4 );
				buff[0] = (BYTE)( w >> 8 );
				buff[1] = (BYTE)( w & 0xff );
				HIDSend( buff, 2 );
				break;
	
			default:
				break;
		}
	}
}

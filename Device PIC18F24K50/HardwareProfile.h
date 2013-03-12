#ifndef HARDWARE_PROFILE_H
#define HARDWARE_PROFILE_H

// ************ IMPORTANT ************
// Modify to your own VID/PID
#define USB_VID        0x04D8
#define USB_PID        0x003F
#define USB_RELEASE_NO	0x0000

#define USB_LANG_CODE	0x0409
#define USB_MFR_STR	'O','a','k','t','r','e','e','-','l','a', 'b','.'
#define USB_PROD_STR	'H','I','D',' ','S','a','m','p','l','e'

#define self_power 0
#undef USE_USB_BUS_SENSE_IO

#define WAIT_UNTIL_ACT_LOCK

#endif

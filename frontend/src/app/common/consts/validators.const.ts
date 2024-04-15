import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function validateUrl(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
        const url: string = control.value;

        if (!url) {
            return null;
        }

        const urlRegex = new RegExp(/\b(?:[Hh][Tt][Tt][Pp][Ss]?):\/\/[-A-Za-z0-9+&@#\/%?=~_|!:,.;]*[-A-Za-z0-9+&@#\/%=~_|]/);
        const validUrl = urlRegex.test(url.toLowerCase());

        return validUrl ? null : { invalidUrl: true };
    };
}

export function validatePhone(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
        const phone: string = control.value;

        if (!phone) {
            return null;
        }

        const phoneRegex = new RegExp(/^\(\d{3}\) \d{3}-\d{4}$/);
        const validUrl = phoneRegex.test(phone);

        return validUrl ? null : { invalidPhone: true };
    };
}

export function validateEmailListString(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
        let emailList;

        if (!!control.value) {
            emailList = (control.value as string).split(',').filter(x => !!x).map(x => x.trim()) as Array<string>;
        }

        if (!emailList || !emailList.length) {
            return null;
        }

        const emailRegex = new RegExp(/^(?=.{1,254}$)(?=.{1,64}@)[-!#$%&'*+/0-9=?A-Z^_`a-z{|}~]+(\.[-!#$%&'*+/0-9=?A-Z^_`a-z{|}~]+)*@[A-Za-z0-9]([A-Za-z0-9-]{0,61}[A-Za-z0-9])?(\.[A-Za-z0-9]([A-Za-z0-9-]{0,61}[A-Za-z0-9])?)*$/);
        const allValid = emailList.every((email: string) => {
            return emailRegex.test(email.toLowerCase())
        })

        return allValid ? null : { invalidEmailList: true };
    };
}
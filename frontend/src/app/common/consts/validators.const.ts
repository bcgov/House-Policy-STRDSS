import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function validateUrl(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
        const url = control.value;

        if (!url) {
            return null;
        }

        const urlRegex = new RegExp(/(https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|www\.[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9]+\.[^\s]{2,}|www\.[a-zA-Z0-9]+\.[^\s]{2,})/);
        const validUrl = urlRegex.test(url);

        return validUrl ? null : { invalidUrl: true };
    };
}

export function validatePhone(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
        const url = control.value;

        if (!url) {
            return null;
        }

        const phoneRegex = new RegExp(/^\(\d{3}\) \d{3}-\d{4}$/);
        const validUrl = phoneRegex.test(url);

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
            return emailRegex.test(email)
        })

        return allValid ? null : { invalidEmailList: true };
    };
}
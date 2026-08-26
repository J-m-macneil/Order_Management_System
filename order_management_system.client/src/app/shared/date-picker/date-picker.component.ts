import { AfterViewInit, Component, ElementRef, EventEmitter, forwardRef, Input, OnDestroy, Output, ViewChild } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import flatpickr from 'flatpickr';
import { Instance } from 'flatpickr/dist/types/instance';

@Component({
  selector: 'app-date-picker',
  standalone: false,
  templateUrl: './date-picker.component.html',
  styleUrls: ['./date-picker.component.css'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => DatePickerComponent),
      multi: true
    }
  ]
})
export class DatePickerComponent implements AfterViewInit, OnDestroy, ControlValueAccessor {
  @Input() ariaLabel = 'Date';
  @Output() dateChange = new EventEmitter<string>();
  @ViewChild('input', { static: true }) input!: ElementRef<HTMLInputElement>;

  private picker?: Instance;
  private value = '';
  private onChange: (value: string) => void = () => {};
  private onTouched: () => void = () => {};

  ngAfterViewInit(): void {
    this.picker = flatpickr(this.input.nativeElement, {
      altInput: true,
      altInputClass: 'form-input app-date-picker-input',
      altFormat: 'd/m/Y',
      ariaDateFormat: 'd/m/Y',
      dateFormat: 'Y-m-d',
      defaultDate: this.value || undefined,
      disableMobile: true,
      allowInput: false,
      onChange: (_dates, value) => {
        this.value = value;
        this.onChange(value);
        this.dateChange.emit(value);
      },
      onClose: () => this.onTouched()
    });
  }

  ngOnDestroy(): void {
    this.picker?.destroy();
  }

  writeValue(value: string | null): void {
    this.value = value ?? '';
    this.picker?.setDate(this.value, false, 'Y-m-d');
  }

  registerOnChange(onChange: (value: string) => void): void {
    this.onChange = onChange;
  }

  registerOnTouched(onTouched: () => void): void {
    this.onTouched = onTouched;
  }

  setDisabledState(disabled: boolean): void {
    this.input.nativeElement.disabled = disabled;

    if (this.picker?.altInput) {
      this.picker.altInput.disabled = disabled;
    }
  }
}

import { Injectable } from '@angular/core';
import { ListingFilter } from '../models/listing-filter';

@Injectable({
  providedIn: 'root'
})
export class FilterPersistenceService {
  listingFilter!: ListingFilter;
  constructor() { }
}

import { Injectable } from '@angular/core';

const STORAGE_KEY = 'strdss_visited_rental_listing_ids';

@Injectable({
  providedIn: 'root',
})
export class VisitedListingsSessionService {
  private ids = new Set<number>();
  private loaded = false;

  markVisited(rentalListingId: number): void {
    if (!Number.isFinite(rentalListingId)) {
      return;
    }
    this.ensureLoaded();
    this.ids.add(rentalListingId);
    this.persist();
  }

  isVisited(rentalListingId: number): boolean {
    if (!Number.isFinite(rentalListingId)) {
      return false;
    }
    this.ensureLoaded();
    return this.ids.has(rentalListingId);
  }

  private ensureLoaded(): void {
    if (this.loaded) {
      return;
    }
    this.loaded = true;
    if (typeof sessionStorage === 'undefined') {
      return;
    }
    try {
      const raw = sessionStorage.getItem(STORAGE_KEY);
      if (!raw) {
        return;
      }
      const parsed = JSON.parse(raw) as unknown;
      if (!Array.isArray(parsed)) {
        sessionStorage.removeItem(STORAGE_KEY);
        return;
      }
      for (const item of parsed) {
        if (typeof item === 'number' && Number.isFinite(item)) {
          this.ids.add(item);
        } else if (typeof item === 'string' && /^\d+$/.test(item)) {
          this.ids.add(Number(item));
        }
      }
    } catch {
      try {
        sessionStorage.removeItem(STORAGE_KEY);
      } catch {
        /* ignore */
      }
      this.ids = new Set();
    }
  }

  private persist(): void {
    if (typeof sessionStorage === 'undefined') {
      return;
    }
    try {
      sessionStorage.setItem(STORAGE_KEY, JSON.stringify([...this.ids]));
    } catch {
      /* quota / private mode */
    }
  }
}

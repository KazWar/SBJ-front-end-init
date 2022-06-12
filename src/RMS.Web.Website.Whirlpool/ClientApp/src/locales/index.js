import enUs from './en-us.json';
import enGb from './en-gb.json'
import nl from './nl-nl.json';
import beFr from './be-fr.json';
import beNl from './be-nl.json';

export const defaultLocale = 'en';

/* eslint-disable @typescript-eslint/camelcase */
export const locales = {
  en_us: enUs,
  en_gb: enGb,
  nl_nl: nl,
  be_fr: beFr,
  be_nl: beNl
}
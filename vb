RestrictionFetcher fetcher = new RestrictionFetcher();
List<RestrictedSecurity> restrictions = fetcher.fetchRestrictions(connection);

for (RestrictedSecurity restriction : restrictions) {
    System.out.println(restriction);
}

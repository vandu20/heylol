    public void generateXMLReport(String restrictionType) {
        try {
            // Simulated XMLStreamWriter (Replace this with actual writer)
            XMLStreamWriter xtw = new StaXBuilder(); 

            LOGGER.info("Generating XML report...");

            StaXBuilder staXBuilder = new StaXBuilder(xtw);

            // Create XML report details
            generateReportDetails(staXBuilder, restrictionType);

            // Simulating restricted securities list
            List<String> restrictions = List.of("Sec1", "Sec2", "Sec3");  
            generaterestrictedSecuritiesList(staXBuilder, restrictions);

            staXBuilder.endElement(); // Close "restrictedSecurityList" element
            LOGGER.info("Report generation completed successfully");

        } catch (Exception e) {
            LOGGER.error("Error while generating XML report", e);
        }
    }

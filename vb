protected String generateRSReport(String restrictionType, String overrideRestrictionTypeName) {
    LOGGER.info("Generate RS Report");

    ByteArrayOutputStream extractorOutput = null;
    InputStream is = null;
    String filePath = "src/main/resources/restricted_securities.xml";

    try {
        // Extract data
        extractorOutput = rsExtractor.extract(restrictionType, overrideRestrictionTypeName);
        
        // Convert output stream to input stream
        is = new ByteArrayInputStream(extractorOutput.toByteArray());

        // Save to file
        Path path = Paths.get(filePath);
        Files.createDirectories(path.getParent()); // Ensure the directory exists
        Files.copy(is, path, StandardCopyOption.REPLACE_EXISTING);

        LOGGER.info("Report saved at: " + filePath);
        
    } catch (IOException e) {
        LOGGER.error("Error while saving the report: " + e.getMessage(), e);
    } finally {
        try {
            if (extractorOutput != null) {
                extractorOutput.close();
            }
            if (is != null) {
                is.close();
            }
        } catch (IOException e) {
            LOGGER.error("Error closing streams: " + e.getMessage(), e);
        }
    }
    
    return filePath;
}

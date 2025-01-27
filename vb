package com.example.dbcruise.service;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;
import org.springframework.web.client.RestTemplate;
import org.springframework.web.client.HttpClientErrorException;
import org.springframework.web.client.HttpServerErrorException;

@Service
public class DbCruiseService {
    private final RestTemplate restTemplate;

    @Value("${dbcruise.service.url}")
    private String serviceUrl;

    public DbCruiseService(RestTemplate restTemplate) {
        this.restTemplate = restTemplate;
    }

    public String getSnapshot() {
        try {
            // Fetching the response from the API as a String
            return restTemplate.getForObject(serviceUrl, String.class);
        } catch (HttpClientErrorException | HttpServerErrorException e) {
            // Handling HTTP exceptions with specific status codes
            return "HTTP Error: " + e.getStatusCode() + " - " + e.getResponseBodyAsString();
        } catch (Exception e) {
            // Handling all other exceptions
            return "Error occurred: " + e.getMessage();
        }
    }
}

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package barney.astoncounty;

import astonblades.Application.Randomiser;
import astonblades.GUI.AllCharactersPanel;
import astonblades.GUI.GamesMasterPanel;
import astonblades.GUI.ImageProcessor;
import javafx.application.Application;
import static javafx.application.Application.launch;
import javafx.scene.Scene;
import javafx.scene.layout.HBox;
import javafx.stage.Stage;

/**
 *
 * @author hp
 */
public class AstonBlades extends Application
{

    public void start(Stage stage) throws Exception 
    {
        double stageHeight = 1000;
        double stageWidth = 1800;
        double panelHeight = stageHeight;
        double panelWidth = stageWidth / 5;
        stage.setTitle("The Horrors of Aston County");
        stage.setHeight(stageHeight);
        stage.setWidth(stageWidth);
        
        Randomiser.InitialiseRandomGenerator();
        ImageProcessor.LoadAllImages();
        
        HBox hb = new HBox();  
        hb.setStyle("-fx-font-size: 12pt;\n" +
            "-fx-background-color: black;\n" +
            "-fx-text-fill: white;\n" +
            "-fx-border-color: white;\n" +
            "-fx-border-width: 1;\n" +
            "-fx-border-style: solid;\n");
        
        AllCharactersPanel acp = new AllCharactersPanel(panelHeight, panelWidth * 4);
        hb.getChildren().addAll(new GamesMasterPanel(panelHeight, panelWidth, acp), acp);
        
        Scene scene = new Scene(hb);
        stage.setScene(scene);
        stage.show();
    }
    
    public static void run(String[] args) {
        launch(args);
    }    
    
}
